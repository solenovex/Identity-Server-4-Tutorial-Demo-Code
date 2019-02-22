import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { TodoTableComponent } from './components/todo-table/todo-table.component';
import { AddTodoComponent } from './components/add-todo/add-todo.component';
import { SigninOidcComponent } from './oidc/signin-oidc/signin-oidc.component';
import { RedirectSilentRenewComponent } from './oidc/redirect-silent-renew/redirect-silent-renew.component';

const routes: Routes = [
  {
    path: 'dashboard',
    component: DashboardComponent
  },
  {
    path: 'todo',
    component: TodoTableComponent
  },
  {
    path: 'todo-add',
    component: AddTodoComponent
  },
  {
    path: 'signin-oidc',
    component: SigninOidcComponent
  },
  {
    path: 'redirect-silentrenew',
    component: RedirectSilentRenewComponent
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
