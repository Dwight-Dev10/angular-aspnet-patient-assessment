import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { PatientService } from '../../services/patient.service';

@Component({
  selector: 'app-patient-form',
  imports: [ReactiveFormsModule],
  templateUrl: './patient-form.html',
})
export class PatientForm {
  private readonly fb = inject(FormBuilder);
  private readonly patientService = inject(PatientService);

  readonly form = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    dateOfBirth: ['', Validators.required],
    condition: [''],
  });

  onSubmit(): void {
    // TODO: send the new patient to the API
  }
}
