import { Component, OnInit } from '@angular/core';
import { TodoService } from 'src/app/services/todo.service';
import { ITodo } from 'src/app/models/todo';

@Component({
  selector: 'ac-todo-table',
  templateUrl: './todo-table.component.html',
  styleUrls: ['./todo-table.component.scss']
})
export class TodoTableComponent implements OnInit {
  todos: ITodo[];

  displayedColumns = ['id', 'title', 'completed'];

  constructor(private todoService: TodoService) {}

  ngOnInit() {
    this.todoService.getAllTodos().subscribe(todos => {
      this.todos = todos;
      console.log(this.todos);
    });
  }
}
