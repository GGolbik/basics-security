import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ApiDocComponent } from './api-doc.component';

const routes: Routes = [{
  path: '', component: ApiDocComponent, children: [
    { path: '', component: ApiDocComponent }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ApiDocRoutingModule { }
