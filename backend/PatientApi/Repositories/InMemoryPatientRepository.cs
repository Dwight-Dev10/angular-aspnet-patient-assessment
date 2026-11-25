using PatientApi.Models;

namespace PatientApi.Repositories;

public class InMemoryPatientRepository : IPatientRepository
{
    private readonly List<Patient> _patients =
    [
        new() { Id = 1, FirstName = "Ada",   LastName = "Lovelace", DateOfBirth = new DateOnly(1985, 12, 10), Condition = "Hypertension" },
        new() { Id = 2, FirstName = "Alan",  LastName = "Turing",   DateOfBirth = new DateOnly(1990, 6, 23),  Condition = "Asthma" },
        new() { Id = 3, FirstName = "Grace", LastName = "Hopper",   DateOfBirth = new DateOnly(1978, 12, 9),  Condition = "Type 2 Diabetes" },
    ];

    private int _nextId = 4;

    public Task<IReadOnlyList<Patient>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<Patient>>(_patients.ToList());

    public Task<Patient?> GetByIdAsync(int id) =>
        Task.FromResult(_patients.FirstOrDefault(p => p.Id == id));

    public Task<Patient> AddAsync(Patient patient)
    {
        patient.Id = _nextId++;
        _patients.Add(patient);
        return Task.FromResult(patient);
    }
}
