import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DocRoutingModule } from './doc-routing.module';
import { DocComponent } from './doc.component';
import { LicenseDialogComponent } from './license-dialog/license-dialog.component';
import { MaterialModule } from '../material.module';


@NgModule({
  declarations: [
    DocComponent,
    LicenseDialogComponent
  ],
  imports: [
    CommonModule,
    DocRoutingModule,
    MaterialModule
  ]
})
export class DocModule { }
