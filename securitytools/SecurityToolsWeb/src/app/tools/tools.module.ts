import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ToolsRoutingModule } from './tools-routing.module';
import { ToolsComponent } from './tools.component';
import { DisplayDialogComponent } from '../display-dialog/display-dialog.component';
import { CrlEntriesComponent } from './elements/crl-entries/crl-entries.component';
import { CrlEntryComponent } from './elements/crl-entry/crl-entry.component';
import { CrlValidityComponent } from './elements/crl-validity/crl-validity.component';
import { AuthorityKeyIdentifierComponent } from './elements/extensions/authority-key-identifier/authority-key-identifier.component';
import { BasicConstraintsComponent } from './elements/extensions/basic-constraints/basic-constraints.component';
import { ExtendedKeyUsageComponent } from './elements/extensions/extended-key-usage/extended-key-usage.component';
import { ExtensionsComponent } from './elements/extensions/extensions.component';
import { KeyUsageComponent } from './elements/extensions/key-usage/key-usage.component';
import { SubjectAlternativeNameComponent } from './elements/extensions/subject-alternative-name/subject-alternative-name.component';
import { SubjectKeyIdentifierComponent } from './elements/extensions/subject-key-identifier/subject-key-identifier.component';
import { FileEntriesComponent } from './elements/file-entries/file-entries.component';
import { HashAlgorithmComponent } from './elements/hash-algorithm/hash-algorithm.component';
import { IssuerComponent } from './elements/issuer/issuer.component';
import { PrivatekeyComponent } from './elements/privatekey/privatekey.component';
import { SubjectnameComponent } from './elements/subjectname/subjectname.component';
import { ValidityComponent } from './elements/validity/validity.component';
import { ExecuteDialogComponent } from './execute-dialog/execute-dialog.component';
import { CertificateConfigComponent } from './sections/certificate-config/certificate-config.component';
import { CrlComponent } from './sections/crl/crl.component';
import { CsrConfigComponent } from './sections/csr-config/csr-config.component';
import { KeyPairComponent } from './sections/key-pair/key-pair.component';
import { SelfCertificateConfigComponent } from './sections/self-certificate-config/self-certificate-config.component';
import { TransformComponent } from './sections/transform/transform.component';
import { MaterialModule } from '../material.module';
import { FormsModule } from '@angular/forms';
import { CoreModule } from '../core/core.module';


@NgModule({
  declarations: [
    ToolsComponent,
    CertificateConfigComponent,
    CsrConfigComponent,
    SelfCertificateConfigComponent,
    SubjectnameComponent,
    ValidityComponent,
    PrivatekeyComponent,
    BasicConstraintsComponent,
    KeyUsageComponent,
    ExtendedKeyUsageComponent,
    SubjectAlternativeNameComponent,
    ExtensionsComponent,
    SubjectKeyIdentifierComponent,
    AuthorityKeyIdentifierComponent,
    ExecuteDialogComponent,
    KeyPairComponent,
    IssuerComponent,
    CrlComponent,
    CrlValidityComponent,
    HashAlgorithmComponent,
    CrlEntriesComponent,
    CrlEntryComponent,
    TransformComponent,
    FileEntriesComponent,
    DisplayDialogComponent,
  ],
  imports: [
    CommonModule,
    ToolsRoutingModule,
    MaterialModule,
    FormsModule,
    CoreModule
  ]
})
export class ToolsModule { }
