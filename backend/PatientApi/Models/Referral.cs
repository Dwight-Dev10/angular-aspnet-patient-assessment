namespace PatientApi.Models;

public class Referral
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string Specialty { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}
