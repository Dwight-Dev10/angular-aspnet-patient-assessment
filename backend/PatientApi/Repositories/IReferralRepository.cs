using PatientApi.Models;

namespace PatientApi.Repositories;

public interface IReferralRepository
{
    Task<Referral> AddAsync(Referral referral);
    Task<IReadOnlyList<Referral>> GetByPatientAsync(int patientId);
}
