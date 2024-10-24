import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DocComponent } from './doc.component';

const routes: Routes = [{
  path: '', component: DocComponent, children: [
    { path: '', component: DocComponent }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DocRoutingModule { }
