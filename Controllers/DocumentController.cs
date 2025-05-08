using Microsoft.AspNetCore.Mvc;
using ValidationAPI.Models;
using ValidationAPI.Services;
using ValidationAPI.Repositories;

namespace ValidationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly MetadataService _metadataService;
        private readonly ValidationRepository _validationRepository;
        private readonly NotificationService _notificationService;

        public DocumentController(
            MetadataService metadataService,
            ValidationRepository validationRepository,
            NotificationService notificationService)
        {
            _metadataService = metadataService;
            _validationRepository = validationRepository;
            _notificationService = notificationService;
        }

        [HttpPost("upload")]
        public async Task<ActionResult> UploadDocument([FromForm] IFormFile file, [FromForm] string docType)
        {
            try
            {
                var metadata = await _metadataService.ExtractMetadataAsync(file);
                var record = new DocumentValidation
                {
                    UploadId = metadata.UploadId,
                    DocumentType = docType,
                    FileName = metadata.FileName,
                    ContentType = metadata.ContentType,
                    FileSize = metadata.FileSize,
                    CaptureDate = metadata.CaptureDate,
                    Manufacturer = metadata.Manufacturer,
                    Model = metadata.Model,
                    GpsLocation = metadata.GpsLocation,
                    Status = SimulateFraudDetection(file) ? ValidationStatus.FraudSuspected : ValidationStatus.PendingReview
                };

                await _validationRepository.InsertDocumentAsync(record);

                if (record.Status == ValidationStatus.FraudSuspected)
                {
                    var uploadGuid = Guid.Parse(record.UploadId);
                    await _notificationService.NotifyFraudAsync(uploadGuid);
                }

                return Ok(new { uploadId = record.UploadId, status = record.Status == ValidationStatus.FraudSuspected ? "rejected" : "accepted" });
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
