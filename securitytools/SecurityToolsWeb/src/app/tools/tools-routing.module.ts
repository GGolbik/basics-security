import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ToolsComponent } from './tools.component';
import { KeyPairComponent } from './sections/key-pair/key-pair.component';
import { TransformComponent } from './sections/transform/transform.component';
import { CrlComponent } from './sections/crl/crl.component';
import { SelfCertificateConfigComponent } from './sections/self-certificate-config/self-certificate-config.component';
import { CertificateConfigComponent } from './sections/certificate-config/certificate-config.component';
import { CsrConfigComponent } from './sections/csr-config/csr-config.component';

const routes: Routes = [{
  path: '', component: ToolsComponent, children: [
    { path: '', component: KeyPairComponent },
    { path: 'keypair', component: KeyPairComponent },
    { path: 'csr', component: CsrConfigComponent },
    { path: 'certificate', component: CertificateConfigComponent },
    { path: 'selfsigncertificate', component: SelfCertificateConfigComponent },
    { path: 'crl', component: CrlComponent },
    { path: 'transform', component: TransformComponent }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ToolsRoutingModule { }
