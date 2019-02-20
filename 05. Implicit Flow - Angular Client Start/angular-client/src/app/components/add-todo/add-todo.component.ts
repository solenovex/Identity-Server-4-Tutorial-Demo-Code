import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { TodoService } from 'src/app/services/todo.service';
import { MatSnackBar } from '@angular/material';
import { Router } from '@angular/router';

@Component({
  selector: 'ac-add-todo',
  templateUrl: './add-todo.component.html',
  styleUrls: ['./add-todo.component.css']
})
export class AddTodoComponent {
  addressForm = this.fb.group({
    title: [null, Validators.required]
  });

  constructor(private fb: FormBuilder, private todoService: TodoService, private snackBar: MatSnackBar, private router: Router) {}

  onSubmit() {
    if (this.addressForm.valid) {
      const todo = this.addressForm.value;
      this.todoService.addTodo(todo).subscribe(td => {
        this.snackBar.open('Successful!', 'Close', { duration: 5000 });
        this.router.navigate(['/todo']);
      });
    }
  }
}
