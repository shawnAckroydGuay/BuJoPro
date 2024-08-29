import { Component } from '@angular/core';
import { MONTHS } from 'src/ressources/Months';

@Component({
  selector: 'app-month-input',
  templateUrl: './month-input.component.html',
  styleUrls: ['./month-input.component.css']
})
export class MonthInputComponent 
{
  listMonth = MONTHS;
}
