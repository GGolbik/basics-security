import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListComponent } from './list/list.component';
import { UpdateComponent } from './update/update.component';
import { CredentialsComponent } from './credentials.component';

const routes: Routes = [{ path: '', component: CredentialsComponent, children: [
    { path: '', component: ListComponent },
    { path: 'list', component: ListComponent },
    { path: 'list/:id', component: UpdateComponent }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CredentialsRoutingModule { }
