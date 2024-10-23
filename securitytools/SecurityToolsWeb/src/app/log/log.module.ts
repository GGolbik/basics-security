import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LogRoutingModule } from './log-routing.module';
import { LogComponent } from './log.component';
import { MaterialModule } from '../material.module';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';


@NgModule({
  declarations: [
    LogComponent
  ],
  imports: [
    CommonModule,
    LogRoutingModule,
    MaterialModule,
    HttpClientModule,
    FormsModule,
  ]
})
export class LogModule { }
