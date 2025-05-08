using MongoDB.Driver;
using ValidationAPI.Models;
using System.Threading.Tasks;

namespace ValidationAPI.Repositories
{
    public class ValidationRepository : IValidationRepository
    {
        private readonly IMongoCollection<BiometricValidation> _biometricRecords;
        private readonly IMongoCollection<DocumentValidation> _documentRecords;

        public ValidationRepository(IConfiguration configuration)
        {
            var mongoClient = new MongoClient(configuration.GetValue<string>("MongoSettings:ConnectionString"));
            var database = mongoClient.GetDatabase(configuration.GetValue<string>("MongoSettings:DatabaseName"));
            _biometricRecords = database.GetCollection<BiometricValidation>("biometricValidations");
            _documentRecords = database.GetCollection<DocumentValidation>("documentValidations");
        }

        public async Task<BiometricValidation> InsertBiometricAsync(BiometricValidation record)
        {
            await _biometricRecords.InsertOneAsync(record);
            return record;
        }

        public async Task<DocumentValidation> InsertDocumentAsync(DocumentValidation record)
        {
            await _documentRecords.InsertOneAsync(record);
            return record;
        }

        public async Task UpdateBiometricAsync(string uploadId, ValidationStatus status, bool notified)
        {
            var filter = Builders<BiometricValidation>.Filter.Eq(r => r.UploadId, uploadId);
            var update = Builders<BiometricValidation>.Update
                .Set(r => r.Status, status)
                .Set(r => r.Notified, notified)
                .Set(r => r.BiometricTimestamp, DateTime.UtcNow);

            await _biometricRecords.UpdateOneAsync(filter, update);
        }

        public async Task UpdateDocumentAsync(string uploadId, ValidationStatus status, bool notified)
        {
            var filter = Builders<DocumentValidation>.Filter.Eq(r => r.UploadId, uploadId);
            var update = Builders<DocumentValidation>.Update
                .Set(r => r.Status, status)
                .Set(r => r.Notified, notified)
                .Set(r => r.DocumentTimestamp, DateTime.UtcNow);

            await _documentRecords.UpdateOneAsync(filter, update);
        }

        public async Task<BiometricValidation> GetBiometricByIdAsync(string uploadId)
        {
            var filter = Builders<BiometricValidation>.Filter.Eq(r => r.UploadId, uploadId);
            return await _biometricRecords.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<DocumentValidation> GetDocumentByIdAsync(string uploadId)
        {
            var filter = Builders<DocumentValidation>.Filter.Eq(r => r.UploadId, uploadId);
            return await _documentRecords.Find(filter).FirstOrDefaultAsync();
        }
    }
}
