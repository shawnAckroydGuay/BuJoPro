import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MonthInputComponent } from '../month-input/month-input.component';


@Component({
  selector: 'app-user-input',
  templateUrl: './user-input.component.html',
  styleUrls: ['./user-input.component.css']
})
export class UserInputComponent {
  constructor(private http: HttpClient) {}

  downloadPdf() {
    this.http.get('http://localhost:80/BuJoPro/GetBuJoPDFV1', { responseType: 'blob' })
      .subscribe((response: Blob) => {
        const url = window.URL.createObjectURL(response);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'document.pdf';
        a.click();
        window.URL.revokeObjectURL(url);
      });
  }
}