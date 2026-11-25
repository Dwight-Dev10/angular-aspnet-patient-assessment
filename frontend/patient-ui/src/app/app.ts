import { Component } from '@angular/core';
import { PatientForm } from './components/patient-form/patient-form';
import { PatientList } from './components/patient-list/patient-list';

@Component({
  selector: 'app-root',
  imports: [PatientList, PatientForm],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {}
