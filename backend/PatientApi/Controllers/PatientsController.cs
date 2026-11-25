using Microsoft.AspNetCore.Mvc;
using PatientApi.Dtos;
using PatientApi.Services;

namespace PatientApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController(IPatientService patients) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await patients.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => Ok(await patients.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create(CreatePatientRequest request) =>
        Ok(await patients.CreateAsync(request));
}
