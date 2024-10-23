import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { replaceAll, escapeHtml } from '../../../main';

@Component({
  selector: 'app-license-dialog',
  templateUrl: './license-dialog.component.html',
  styleUrls: ['./license-dialog.component.css']
})
export class LicenseDialogComponent implements OnInit {

  /**
   * counter to know whether there is some work in progress.
   */
  protected loadingCount: number = 0;

  /**
   * The license text of UI 3rd-party in html
   */
  protected frontendLicensesHtml: string = "";

  constructor(private http: HttpClient, @Inject("BASE_URL") private baseUrl: string) { }

  ngOnInit(): void {
    this.getFrontendLicenses();
  }

  /**
   * Load frontend licenses
   */
  private getFrontendLicenses() {
    this.frontendLicensesHtml = "";
    this.loadingCount++;
    this.http.get(this.baseUrl + 'licenses.txt', { responseType: 'text' }).subscribe({
      next: (result) => {
        this.loadingCount--;
        this.frontendLicensesHtml += replaceAll(escapeHtml(result), '\n', '<br>');
      },
      error: (error) => {
        this.loadingCount--;
        console.error(error);
        // don't show error to user
      }
    });
    this.http.get(this.baseUrl + '3rdpartylicenses.txt', { responseType: 'text' }).subscribe({
      next: (result) => {
        this.loadingCount--;
        this.frontendLicensesHtml += replaceAll(escapeHtml(result), '\n', '<br>');
      },
      error: (error) => {
        this.loadingCount--;
        console.error(error);
        // don't show error to user
      }
    });
  }

}
