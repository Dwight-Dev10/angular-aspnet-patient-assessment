# Patient Referrals — Practice Code Assessment

**Stack:** Angular 21 (standalone, signals, zoneless) · ASP.NET Core 10 Web API (controllers, DI)
**Time:** ~60–90 minutes · **Answers:** `ANSWER_KEY.md` (don't peek until you're done)

## The scenario

A small clinic app lets staff **see patients**, **add a patient**, and **refer a patient** to a
specialty. A teammate scaffolded it and left. It *builds*, but it doesn't really work, and some
pieces were never finished.

## Run it

```bash
# Terminal 1 — API on http://localhost:5006
cd backend/PatientApi
dotnet run --launch-profile http

# Terminal 2 — UI on http://localhost:4200
cd frontend/patient-ui
npm install
npm start
```

`backend/PatientApi/PatientApi.http` has ready-made requests you can send from VS Code / Rider.

## Your job

Make these three flows work end to end, then make the code something you'd be happy to ship:

1. The patient list loads from the API and displays correctly (name, date of birth, condition).
2. Adding a patient saves it, and it shows up in the list.
3. Referring a patient sends a referral to the API and closes the form.

Along the way, **find and fix what's wrong** — on both sides. Talk through what you find
(or write notes) as you go; explaining *why* something is broken counts as much as fixing it.

Rules of thumb:
- Keep the existing structure; don't rewrite it from scratch.
- Prefer small, idiomatic fixes (Angular signals / modern control flow, ASP.NET DI and REST conventions).
- An in-memory store is fine — no database needed.

## Stretch goals (if time allows)

- `GET /api/patients/{id}/referrals` endpoint, and show a referral count per patient.
- Add one unit test on each side (a service test in .NET, a component or service test in Angular).
- Disable the submit buttons while a request is in flight.

## Layout

```
backend/PatientApi/     Controllers · Services · Repositories · Models · Dtos · Program.cs
frontend/patient-ui/    src/app/{components,services,models}
```
