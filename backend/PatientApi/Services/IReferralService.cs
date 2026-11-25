using PatientApi.Dtos;
using PatientApi.Models;

namespace PatientApi.Services;

public interface IReferralService
{
    Task<Referral> CreateAsync(CreateReferralRequest request);
    Task<IReadOnlyList<Referral>> GetByPatientAsync(int patientId);
}
