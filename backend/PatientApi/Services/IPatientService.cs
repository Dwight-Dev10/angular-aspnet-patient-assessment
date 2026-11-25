using PatientApi.Dtos;
using PatientApi.Models;

namespace PatientApi.Services;

public interface IPatientService
{
    Task<IReadOnlyList<Patient>> GetAllAsync();
    Task<Patient?> GetByIdAsync(int id);
    Task<Patient> CreateAsync(CreatePatientRequest request);
}
