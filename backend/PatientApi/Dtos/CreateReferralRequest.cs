namespace PatientApi.Dtos;

public class CreateReferralRequest
{
    public int PatientId { get; set; }
    public string Specialty { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
