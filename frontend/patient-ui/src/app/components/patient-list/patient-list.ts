import { DatePipe } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { Patient } from '../../models/patient.model';
import { PatientService } from '../../services/patient.service';
import { ReferralForm } from '../referral-form/referral-form';

@Component({
  selector: 'app-patient-list',
  imports: [DatePipe, ReferralForm],
  templateUrl: './patient-list.html',
})
export class PatientList implements OnInit {
  private readonly patientService = inject(PatientService);

  patients: Patient[] = [];
  selected: Patient | null = null;

  ngOnInit(): void {
    this.patientService.getPatients().subscribe((patients) => {
      this.patients = patients;
    });
  }

  refer(patient: Patient): void {
    this.selected = patient;
  }

  closeReferral(): void {
    this.selected = null;
  }
}
