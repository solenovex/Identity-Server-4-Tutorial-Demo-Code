import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { TodoTableComponent } from './components/todo-table/todo-table.component';
import { AddTodoComponent } from './components/add-todo/add-todo.component';

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
    path: '**',
    redirectTo: 'dashboard'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
