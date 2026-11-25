using Microsoft.AspNetCore.Mvc;
using PatientApi.Dtos;
using PatientApi.Services;

namespace PatientApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReferralsController(IReferralService referrals) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateReferralRequest request) =>
        Ok(await referrals.CreateAsync(request));
}
