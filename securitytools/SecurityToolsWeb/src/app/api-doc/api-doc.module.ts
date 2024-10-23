import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ApiDocRoutingModule } from './api-doc-routing.module';
import { ApiDocComponent } from './api-doc.component';
import { MaterialModule } from '../material.module';

@NgModule({
  declarations: [
    ApiDocComponent
  ],
  imports: [
    CommonModule,
    ApiDocRoutingModule,
    MaterialModule
  ]
})
export class ApiDocModule { }
