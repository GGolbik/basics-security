import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { DocComponent } from './doc/doc.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from './material.module';
import { CertificateConfigComponent } from './home/sections/certificate-config/certificate-config.component';
import { CsrConfigComponent } from './home/sections/csr-config/csr-config.component';
import { SelfCertificateConfigComponent } from './home/sections/self-certificate-config/self-certificate-config.component';
import { ApiDocComponent } from './api-doc/api-doc.component';
import { SubjectnameComponent } from './home/elements/subjectname/subjectname.component';
import { ValidityComponent } from './home/elements/validity/validity.component';
import { PrivatekeyComponent } from './home/elements/privatekey/privatekey.component';
import { BasicConstraintsComponent } from './home/elements/extensions/basic-constraints/basic-constraints.component';
import { KeyUsageComponent } from './home/elements/extensions/key-usage/key-usage.component';
import { ExtendedKeyUsageComponent } from './home/elements/extensions/extended-key-usage/extended-key-usage.component';
import { SubjectAlternativeNameComponent } from './home/elements/extensions/subject-alternative-name/subject-alternative-name.component';
import { ExtensionsComponent } from './home/elements/extensions/extensions.component';
import { StringListComponent } from './core/string-list/string-list.component';
import { SubjectKeyIdentifierComponent } from './home/elements/extensions/subject-key-identifier/subject-key-identifier.component';
import { AuthorityKeyIdentifierComponent } from './home/elements/extensions/authority-key-identifier/authority-key-identifier.component';
import { ExecuteDialogComponent } from './home/execute-dialog/execute-dialog.component';
import { LogComponent } from './log/log.component';
import { KeyPairComponent } from './home/sections/key-pair/key-pair.component';
import { IssuerComponent } from './home/elements/issuer/issuer.component';
import { CrlComponent } from './home/sections/crl/crl.component';
import { CrlValidityComponent } from './home/elements/crl-validity/crl-validity.component';
import { HashAlgorithmComponent } from './home/elements/hash-algorithm/hash-algorithm.component';
import { CrlEntriesComponent } from './home/elements/crl-entries/crl-entries.component';
import { ByteNumberComponent } from './core/byte-number/byte-number.component';
import { CrlEntryComponent } from './home/elements/crl-entry/crl-entry.component';
import { TransformComponent } from './home/sections/transform/transform.component';
import { FileEntriesComponent } from './home/elements/file-entries/file-entries.component';
import { DisplayDialogComponent } from './display-dialog/display-dialog.component';
import { FileInputComponent } from './core/file-input/file-input.component';
import { LicenseDialogComponent } from './license-dialog/license-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    DocComponent,
    CertificateConfigComponent,
    CsrConfigComponent,
    SelfCertificateConfigComponent,
    ApiDocComponent,
    SubjectnameComponent,
    ValidityComponent,
    PrivatekeyComponent,
    BasicConstraintsComponent,
    KeyUsageComponent,
    ExtendedKeyUsageComponent,
    SubjectAlternativeNameComponent,
    ExtensionsComponent,
    StringListComponent,
    SubjectKeyIdentifierComponent,
    AuthorityKeyIdentifierComponent,
    ExecuteDialogComponent,
    LogComponent,
    KeyPairComponent,
    IssuerComponent,
    CrlComponent,
    CrlValidityComponent,
    HashAlgorithmComponent,
    CrlEntriesComponent,
    ByteNumberComponent,
    CrlEntryComponent,
    TransformComponent,
    FileEntriesComponent,
    DisplayDialogComponent,
    FileInputComponent,
    LicenseDialogComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    MaterialModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'doc', component: DocComponent },
      { path: 'api-doc', component: ApiDocComponent },
      { path: 'log', component: LogComponent },
    ]),
    BrowserAnimationsModule,
    ReactiveFormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
