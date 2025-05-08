using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ValidationAPI.Models
{
    public class DocumentValidation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ValidationId { get; set; } = string.Empty;

        public required string UploadId { get; set; }
        public DateTime DocumentTimestamp { get; set; }
        public required string DocumentType { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime CaptureDate { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string? GpsLocation { get; set; }
        public ValidationStatus Status { get; set; }

        public bool Notified { get; set; }

        public DocumentValidation()
        {
            ValidationId = ObjectId.GenerateNewId().ToString();
            DocumentTimestamp = DateTime.UtcNow;
            Status = ValidationStatus.PendingReview;
            Notified = false;
        }
    }
}
