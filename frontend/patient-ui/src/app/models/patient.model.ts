export interface Patient {
  id: number;
  firstName: string;
  lastName: string;
  dob: string;
  condition: string;
}

export interface CreatePatientRequest {
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  condition: string;
}

export interface CreateReferralRequest {
  patientId: number;
  specialty: string;
  reason: string;
}

export interface Referral extends CreateReferralRequest {
  id: number;
  status: string;
  createdAt: string;
}
