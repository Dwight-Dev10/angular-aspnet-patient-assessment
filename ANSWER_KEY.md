# Answer Key — Patient Referrals Assessment

> Interviewer / self-check only. Candidate README is `README.md`.

There are **11 planted issues** (5 backend, 5 frontend, 1 contract mismatch spanning both) plus stretch goals. Passing bar suggestion:
fix the 3 "blockers" (★) and identify at least 5 others, with correct reasoning.

How the symptoms appear on a fresh run: the page loads with an empty table and a console error
(`NullInjectorError: No provider for HttpClient`). After each fix, the next failure surfaces —
that's intentional, it tests debugging method (read the console and Network tab, then the server log).

| # | Side | Issue | Symptom | Skill shown |
|---|------|-------|---------|-------------|
| 1 ★ | FE | `provideHttpClient()` missing | `NullInjectorError` for `HttpClient` | Angular DI / app config |
| 2 ★ | BE | CORS not configured | Browser blocks calls to `:5006` | Cross-origin, middleware order |
| 3 ★ | BE | `IReferralService` not registered | `POST /api/referrals` → 500 `Unable to resolve service` | ASP.NET DI |
| 4 | BE | Repositories registered **Scoped** | Added patient vanishes on next request | DI lifetimes |
| 5 | FE/BE | Contract mismatch `dob` vs `dateOfBirth` | DOB column blank | Reading Network tab, typing |
| 6 | FE | Plain fields in a **zoneless** app | Data arrives, table stays empty | Signals / change detection |
| 7 | FE | `createReferral` is a stub that throws | Refer button throws | Completing a feature |
| 8 | FE | `PatientForm.onSubmit` is a TODO | Add does nothing | Completing a feature |
| 9 | FE | List not refreshed after add | Need to reload page | Shared state / signals |
| 10 | BE | REST semantics + no validation | `GET /99` → 200 `null`; POST → 200; blank names accepted | HTTP status codes, DataAnnotations |
| 11 | BE | Referral accepted for non-existent patient | Orphan referrals | Business rules, 404 vs 400 |

Bonus observations (not scored, but good signs): hard-coded API URL, no loading/error UI,
no tests, `[ApiController]` auto-400 only works once validation attributes exist.

---

## Backend fixes

### 3 — Register the missing service (`Program.cs`)

```csharp
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IReferralService, ReferralService>();   // ← was missing
```

Why it only fails at request time: controllers are resolved per request, so the container
doesn't complain at startup. Look at the exception text in the `dotnet run` console.

### 4 — Lifetimes (`Program.cs`)

The in-memory repositories **are** the database, so they must outlive a request:

```csharp
builder.Services.AddSingleton<IPatientRepository, InMemoryPatientRepository>();
builder.Services.AddSingleton<IReferralRepository, InMemoryReferralRepository>();
```

Services stay `Scoped`. A Singleton must never depend on a Scoped service (captive dependency) —
that's why the direction matters. Good candidates also mention that the `List<T>` / `_nextId++`
aren't thread-safe under concurrent requests and would use a `lock`, `ConcurrentDictionary`
+ `Interlocked.Increment`, or (in reality) EF Core with a scoped `DbContext`.

### 2 — CORS (`Program.cs`)

```csharp
builder.Services.AddCors(o => o.AddPolicy("Frontend", p => p
    .WithOrigins("http://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod()));
...
app.UseHttpsRedirection();
app.UseCors("Frontend");      // before UseAuthorization / MapControllers
app.UseAuthorization();
```

Alternative accepted answer: an Angular dev-server proxy (`proxy.conf.json`) and a relative `/api` URL.
Rejected answer: `AllowAnyOrigin()` presented as the production solution.

### 10 — REST semantics and validation

`Controllers/PatientsController.cs`:

```csharp
[HttpGet("{id:int}")]
public async Task<IActionResult> GetById(int id) =>
    await patients.GetByIdAsync(id) is { } patient ? Ok(patient) : NotFound();

[HttpPost]
public async Task<IActionResult> Create(CreatePatientRequest request)
{
    var created = await patients.CreateAsync(request);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);   // 201 + Location
}
```

`Dtos/CreatePatientRequest.cs` (same idea for `CreateReferralRequest`):

```csharp
using System.ComponentModel.DataAnnotations;

[Required, StringLength(50)] public string FirstName { get; set; } = string.Empty;
[Required, StringLength(50)] public string LastName  { get; set; } = string.Empty;
```

Because the controllers have `[ApiController]`, invalid models automatically return
`400 ValidationProblemDetails` — no manual `ModelState` check needed. Extra credit: reject a
future `DateOfBirth` (custom `ValidationAttribute` or `IValidatableObject`).

### 11 — Referral must point at a real patient

`ReferralService` now takes `IPatientRepository` and returns `null` when the patient is missing:

```csharp
public class ReferralService(IReferralRepository referrals, IPatientRepository patients) : IReferralService
{
    public async Task<Referral?> CreateAsync(CreateReferralRequest request)
    {
        if (await patients.GetByIdAsync(request.PatientId) is null) return null;

        return await referrals.AddAsync(new Referral
        {
            PatientId = request.PatientId,
            Specialty = request.Specialty,
            Reason = request.Reason,
            CreatedAt = DateTime.UtcNow,
        });
    }
    ...
}
```

(Change `IReferralService.CreateAsync` to return `Task<Referral?>`.) Controller:

```csharp
[HttpPost]
public async Task<IActionResult> Create(CreateReferralRequest request) =>
    await referrals.CreateAsync(request) is { } referral
        ? StatusCode(StatusCodes.Status201Created, referral)
        : NotFound($"Patient {request.PatientId} not found.");
```

404 vs 400 is a fair debate (body references a missing resource → many teams pick 400/422);
accept either with a stated reason.

---

## Frontend fixes

### 1 — `app.config.ts`

```ts
import { provideHttpClient } from '@angular/common/http';

providers: [
  provideBrowserGlobalErrorListeners(),
  provideRouter(routes),
  provideHttpClient(),
]
```

### 5 — Contract mismatch

The API serializes `DateOfBirth` as `dateOfBirth` (camelCase). The `Patient` interface said `dob`.
Fix the interface (`dateOfBirth: string`) and the template (`patient.dateOfBirth | date`).
Fix the **client** to match the API contract — don't rename the API field to suit the UI.

### 6, 9 — Signals and shared state

Angular 21 is **zoneless by default**: assigning to a plain field inside a `subscribe` callback
does not schedule change detection, so the table never repaints. Move state into signals. Putting
the signal in the service also solves issue 9 (the list reacts when a patient is added) without
`@Output` plumbing between sibling components.

`services/patient.service.ts`:

```ts
private readonly _patients = signal<Patient[]>([]);
private readonly _loading  = signal(false);
private readonly _error    = signal<string | null>(null);

readonly patients = this._patients.asReadonly();
readonly loading  = this._loading.asReadonly();
readonly error    = this._error.asReadonly();

load(): void {
  this._loading.set(true);
  this._error.set(null);
  this.http.get<Patient[]>(`${this.baseUrl}/patients`).subscribe({
    next: (patients) => { this._patients.set(patients); this._loading.set(false); },
    error: () => { this._error.set('Could not load patients.'); this._loading.set(false); },
  });
}

addPatient(request: CreatePatientRequest): Observable<Patient> {
  return this.http
    .post<Patient>(`${this.baseUrl}/patients`, request)
    .pipe(tap((created) => this._patients.update((list) => [...list, created])));
}
```

`patient-list.ts`:

```ts
readonly patients = this.patientService.patients;
readonly loading  = this.patientService.loading;
readonly error    = this.patientService.error;
readonly selected = signal<Patient | null>(null);

ngOnInit(): void { this.patientService.load(); }
refer(patient: Patient)  { this.selected.set(patient); }
closeReferral()          { this.selected.set(null); }
```

`patient-list.html` (calls become `patients()`, `selected()`):

```html
@if (loading()) { <p>Loading…</p> }
@if (error()) { <p role="alert">{{ error() }}</p> }
...
@for (patient of patients(); track patient.id) { ... }
...
@if (selected(); as patient) {
  <app-referral-form [patient]="patient" (done)="closeReferral()" />
}
```

Equally valid: `toSignal(getPatients())`, an `rxResource`/`httpResource`, or `ChangeDetectorRef.markForCheck()`
(weakest — fights the framework). Using `async` pipe on an observable also works.

### 8 — `PatientForm.onSubmit`

```ts
onSubmit(): void {
  if (this.form.invalid) return;
  this.patientService
    .addPatient(this.form.getRawValue())
    .subscribe(() => this.form.reset());
}
```

### 7 — `createReferral`

```ts
createReferral(request: CreateReferralRequest): Observable<Referral> {
  return this.http.post<Referral>(`${this.baseUrl}/referrals`, request);
}
```

(`ReferralForm.onSubmit` already calls it and emits `done`.)

### Bonus — config

Move `http://localhost:5006/api` to `src/environments/environment*.ts` or a proxy config,
and handle errors on `addPatient` / `createReferral` (show a message, keep the form open).

---

## Stretch goal sketches

**Referrals by patient** — `IReferralService.GetByPatientAsync` already exists; expose it:

```csharp
// PatientsController (inject IReferralService too)
[HttpGet("{id:int}/referrals")]
public async Task<IActionResult> GetReferrals(int id) =>
    await patients.GetByIdAsync(id) is null ? NotFound() : Ok(await referrals.GetByPatientAsync(id));
```

**Backend test (xUnit)** — verify the rule from issue 11 with fakes/NSubstitute:

```csharp
[Fact]
public async Task CreateAsync_ReturnsNull_WhenPatientMissing()
{
    var service = new ReferralService(new InMemoryReferralRepository(), new InMemoryPatientRepository());
    var result = await service.CreateAsync(new CreateReferralRequest { PatientId = 999, Specialty = "Cardiology", Reason = "x" });
    Assert.Null(result);
}
```

**Frontend test** — `PatientService` with `provideHttpClientTesting()` + `HttpTestingController`:
assert `load()` issues `GET /api/patients` and that the `patients()` signal holds the response.

---

## Scoring rubric (suggested)

| Area | Strong | Weak |
|------|--------|------|
| Debugging method | Reads console → Network tab → server log; fixes one thing at a time | Guess-and-change |
| DI | Explains lifetimes and captive dependencies | "Singleton fixes it" without why |
| HTTP/REST | 201+Location, 404, 400 ProblemDetails | Everything returns 200 |
| Angular modern style | Signals, `@for/@if`, `inject()`, standalone | `ngOnInit` + manual state, `any` |
| Communication | Explains trade-offs, flags what they'd do next | Silent coding |
