using Microsoft.AspNetCore.Mvc;
using ValidationAPI.Models;
using ValidationAPI.Services;
using ValidationAPI.Repositories;

namespace ValidationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BiometryController : ControllerBase
    {
        private readonly MetadataService _metadataService;
        private readonly ValidationRepository _validationRepository;
        private readonly NotificationService _notificationService;
        private readonly IConfiguration _configuration;

        public BiometryController(
            MetadataService metadataService,
            ValidationRepository validationRepository,
            NotificationService notificationService,
            IConfiguration configuration)
        {
            _metadataService = metadataService;
            _validationRepository = validationRepository;
            _notificationService = notificationService;
            _configuration = configuration;
        }

        [HttpPost("upload")]
        public async Task<ActionResult> UploadBiometry([FromForm] IFormFile file, [FromForm] string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return BadRequest(new { errors = new { type = new[] { "The type field is required." } } });
            }

            if (type != "digital" && type != "facial")
            {
                return BadRequest(new { errors = new { type = new[] { "Invalid biometry type. Must be 'digital' or 'facial'" } } });
            }

            try
            {
                if (file.Length == 0)
                {
                    return BadRequest(new { errors = new { file = new[] { "The file field is required." } } });
                }

                // Extract metadata (which includes validation)
                var metadata = await _metadataService.ExtractMetadataAsync(file);
                var record = new BiometricValidation
                {
                    UploadId = metadata.UploadId,
                    BiometricType = type,
                    FileName = metadata.FileName,
                    ContentType = metadata.ContentType,
                    FileSize = metadata.FileSize,
                    CaptureDate = metadata.CaptureDate,
                    Manufacturer = metadata.Manufacturer,
                    Model = metadata.Model,
                    GpsLocation = metadata.GpsLocation,
                    Status = SimulateFraudDetection(file) ? ValidationStatus.FraudSuspected : ValidationStatus.PendingReview
                };

                await _validationRepository.InsertBiometricAsync(record);

                if (record.Status == ValidationStatus.FraudSuspected)
                {
                    var uploadGuid = Guid.Parse(record.UploadId);
                    await _notificationService.NotifyFraudAsync(uploadGuid);
                }

                return Ok(new { uploadId = record.UploadId, status = record.Status == ValidationStatus.FraudSuspected ? "rejected" : "accepted" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { errors = new { file = new[] { ex.Message } } });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        private bool SimulateFraudDetection(IFormFile file)
        {
            // Aqui poderiamos ter uma lógica de detecção de fraude (por exemplo, validar em API externa ou algum SDK/algoritmo de IA)
            return file.Length > 1000000; // Reject files larger than 1MB
        }
    }
}
