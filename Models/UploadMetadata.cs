using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ValidationAPI.Models
{
    public class UploadMetadata
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UploadId { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }

        public DateTime CaptureDate { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string? GpsLocation { get; set; }

        public UploadMetadata()
        {
            UploadId = ObjectId.GenerateNewId().ToString();
        }
    }
}
