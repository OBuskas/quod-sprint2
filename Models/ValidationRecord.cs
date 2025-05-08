using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ValidationAPI.Models
{
    public enum DocumentStatus
    {
        PendingReview,
        Validated,
        Rejected
    }

    public class ValidationRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string UploadId { get; set; }

        public required string FileName { get; set; }
        public required string ContentType { get; set; }
        public long FileSize { get; set; }

        public UploadMetadata? Metadata { get; set; }
        public DocumentStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Notified { get; set; }

        public ValidationRecord()
        {
            Timestamp = DateTime.UtcNow;
            Notified = false;
            Status = DocumentStatus.PendingReview;
        }
    }
}
