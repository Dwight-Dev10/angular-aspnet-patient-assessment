using PatientApi.Repositories;
using PatientApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Application services
builder.Services.AddScoped<IPatientRepository, InMemoryPatientRepository>();
builder.Services.AddScoped<IReferralRepository, InMemoryReferralRepository>();
builder.Services.AddScoped<IPatientService, PatientService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
