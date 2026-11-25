using PatientApi.Dtos;
using PatientApi.Models;
using PatientApi.Repositories;

namespace PatientApi.Services;

public class ReferralService(IReferralRepository referrals) : IReferralService
{
    public Task<Referral> CreateAsync(CreateReferralRequest request) =>
        referrals.AddAsync(new Referral
        {
            PatientId = request.PatientId,
            Specialty = request.Specialty,
            Reason = request.Reason,
            CreatedAt = DateTime.UtcNow,
        });

    public Task<IReadOnlyList<Referral>> GetByPatientAsync(int patientId) =>
        referrals.GetByPatientAsync(patientId);
}
