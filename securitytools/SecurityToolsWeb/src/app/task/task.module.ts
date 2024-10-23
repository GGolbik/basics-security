import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TaskRoutingModule } from './task-routing.module';
import { TaskComponent } from './task.component';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../material.module';

@NgModule({
  declarations: [
    TaskComponent,
  ],
  imports: [
    CommonModule,
    TaskRoutingModule,
    MaterialModule,
    HttpClientModule,
    FormsModule,
  ]
})
export class TaskModule { }
