import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotFoundComponent } from './core/not-found/not-found.component';

const routes: Routes = [
    { path: '', pathMatch: 'full', redirectTo: 'tools' },
    { path: 'assets', component: NotFoundComponent },
    { path: 'certs', component: NotFoundComponent },
    { path: 'credentials', loadChildren: () => import('./credentials/credentials.module').then(m => m.CredentialsModule) }, 
    { path: 'software', component: NotFoundComponent },
    { path: 'tools', loadChildren: () => import('./tools/tools.module').then(m => m.ToolsModule) }, 
    { path: 'tasks', loadChildren: () => import('./task/task.module').then(m => m.TaskModule) }, 
    { path: 'doc', loadChildren: () => import('./doc/doc.module').then(m => m.DocModule) }, 
    { path: 'api-doc', loadChildren: () => import('./api-doc/api-doc.module').then(m => m.ApiDocModule) }, 
    { path: 'log', loadChildren: () => import('./log/log.module').then(m => m.LogModule) }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }