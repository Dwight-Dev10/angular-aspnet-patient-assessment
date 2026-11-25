using PatientApi.Models;

namespace PatientApi.Repositories;

public class InMemoryReferralRepository : IReferralRepository
{
    private readonly List<Referral> _referrals = [];
    private int _nextId = 1;

    public Task<Referral> AddAsync(Referral referral)
    {
        referral.Id = _nextId++;
        _referrals.Add(referral);
        return Task.FromResult(referral);
    }

    public Task<IReadOnlyList<Referral>> GetByPatientAsync(int patientId) =>
        Task.FromResult<IReadOnlyList<Referral>>(_referrals.Where(r => r.PatientId == patientId).ToList());
}
