using PatientApi.Dtos;
using PatientApi.Models;
using PatientApi.Repositories;

namespace PatientApi.Services;

public class PatientService(IPatientRepository repository) : IPatientService
{
    public Task<IReadOnlyList<Patient>> GetAllAsync() => repository.GetAllAsync();

    public Task<Patient?> GetByIdAsync(int id) => repository.GetByIdAsync(id);

    public Task<Patient> CreateAsync(CreatePatientRequest request) =>
        repository.AddAsync(new Patient
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            Condition = request.Condition,
        });
}
