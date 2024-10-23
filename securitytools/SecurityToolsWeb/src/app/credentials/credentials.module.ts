import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CredentialsComponent } from './credentials.component';
import { ListComponent } from './list/list.component';
import { UpdateComponent } from './update/update.component';
import { CredentialsValueComponent } from './credentials-value/credentials-value.component';
import { HttpClientModule } from '@angular/common/http';
import { CredentialsRoutingModule } from './credentials-routing.module';
import { MaterialModule } from '../material.module';
import { CoreModule } from "../core/core.module";
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    CredentialsComponent,
    ListComponent,
    UpdateComponent,
    CredentialsValueComponent
  ],
  imports: [
    CommonModule,
    CredentialsRoutingModule,
    HttpClientModule,
    MaterialModule,
    CoreModule,
    FormsModule
]
})
export class CredentialsModule { }
