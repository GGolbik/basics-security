import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ByteNumberComponent } from './byte-number/byte-number.component';
import { StringListComponent } from './string-list/string-list.component';
import { FileInputComponent } from './file-input/file-input.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { MaterialModule } from '../material.module';
import { FormsModule } from '@angular/forms';
import { EnumKeyValuePipe } from './enum-key-value-pipe';
import { PasswordFieldComponent } from './password-field/password-field.component';



@NgModule({
  declarations: [
    FileInputComponent,
    ByteNumberComponent,
    StringListComponent,
    NotFoundComponent,
    EnumKeyValuePipe,
    PasswordFieldComponent
  ],
  imports: [
    CommonModule,
    MaterialModule,
    FormsModule
  ],
  exports: [
    FileInputComponent,
    ByteNumberComponent,
    StringListComponent,
    NotFoundComponent,
    EnumKeyValuePipe,
    PasswordFieldComponent
  ]
})
export class CoreModule { }
