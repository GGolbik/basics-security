import { Component, Inject, OnInit } from '@angular/core';
import { SecurityToolsService } from '../services/tool.service';
import { ProgramInfo } from '../services/models';
import { LicenseDialogComponent } from '../license-dialog/license-dialog.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-doc',
  templateUrl: './doc.component.html',
  styleUrls: ['./doc.component.css']
})
export class DocComponent implements OnInit {

  programInfo?: ProgramInfo;

  constructor(private licenseDialog: MatDialog, private service: SecurityToolsService, @Inject("BASE_URL") private baseUrl: string) {

  }
  ngOnInit(): void {
    this.service.getInfo().subscribe({
      next: (value) => {
        this.programInfo = value;
      }
    });
  }

  getVersion() {
    if(this.programInfo == null) {
      return "";
    }
    return this.programInfo.major + "." + this.programInfo.minor + "." + this.programInfo.patch + "." + this.programInfo.build;
  }

  showInfo() {
    this.licenseDialog.open(LicenseDialogComponent);
  }

  get3rdPartyLicenses() {
    return this.baseUrl + '3rdpartylicenses.txt';
  }
}
