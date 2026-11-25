import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CreatePatientRequest,
  CreateReferralRequest,
  Patient,
  Referral,
} from '../models/patient.model';

@Injectable({ providedIn: 'root' })
export class PatientService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5006/api';

  getPatients(): Observable<Patient[]> {
    return this.http.get<Patient[]>(`${this.baseUrl}/patients`);
  }

  addPatient(request: CreatePatientRequest): Observable<Patient> {
    return this.http.post<Patient>(`${this.baseUrl}/patients`, request);
  }

  createReferral(request: CreateReferralRequest): Observable<Referral> {
    // TODO: call the referrals endpoint
    throw new Error('Not implemented');
  }
}
