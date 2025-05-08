using System.Threading.Tasks;
using ValidationAPI.Models;

namespace ValidationAPI.Repositories
{
    public interface IValidationRepository
    {
        Task<BiometricValidation> InsertBiometricAsync(BiometricValidation record);
        Task<DocumentValidation> InsertDocumentAsync(DocumentValidation record);
        Task UpdateBiometricAsync(string uploadId, ValidationStatus status, bool notified);
        Task UpdateDocumentAsync(string uploadId, ValidationStatus status, bool notified);
        Task<BiometricValidation> GetBiometricByIdAsync(string uploadId);
        Task<DocumentValidation> GetDocumentByIdAsync(string uploadId);
    }
}
