import { Component, inject, input, output } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Patient } from '../../models/patient.model';
import { PatientService } from '../../services/patient.service';

@Component({
  selector: 'app-referral-form',
  imports: [ReactiveFormsModule],
  templateUrl: './referral-form.html',
})
export class ReferralForm {
  private readonly fb = inject(FormBuilder);
  private readonly patientService = inject(PatientService);

  readonly patient = input.required<Patient>();
  readonly done = output<void>();

  readonly specialties = ['Cardiology', 'Neurology', 'Endocrinology', 'Pulmonology'];

  readonly form = this.fb.nonNullable.group({
    specialty: ['', Validators.required],
    reason: ['', Validators.required],
  });

  onSubmit(): void {
    const { specialty, reason } = this.form.getRawValue();
    this.patientService
      .createReferral({ patientId: this.patient().id, specialty, reason })
      .subscribe(() => this.done.emit());
  }
}
