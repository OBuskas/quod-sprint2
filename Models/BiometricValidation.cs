using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ValidationAPI.Models
{
    public enum ValidationStatus
    {
        PendingReview,
        NoFraudDetected,
        FraudSuspected
    }

    public class BiometricValidation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ValidationId { get; set; } = string.Empty;

        public required string UploadId { get; set; }
        public DateTime BiometricTimestamp { get; set; }
        public required string BiometricType { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime CaptureDate { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string? GpsLocation { get; set; }
        public ValidationStatus Status { get; set; }
        public string? ReviewerId { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string? ReviewComments { get; set; }
        public bool Notified { get; set; }

        public BiometricValidation()
        {
            ValidationId = ObjectId.GenerateNewId().ToString();
            BiometricTimestamp = DateTime.UtcNow;
            Status = ValidationStatus.PendingReview;
            Notified = false;
        }
    }
}
