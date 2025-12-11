import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TaskService } from '../services/task';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './auth.html',
  styleUrl: './auth.css',
})
export class AuthComponent {
  router = inject(Router);
  taskService = inject(TaskService);

  isLoginMode = signal(true);

  // Form model
  login = '';
  password = '';
  email = '';
  repeatPassword = '';

  toggleMode() {
    this.isLoginMode.update(mode => !mode);
  }

  onSubmit() {
    if (this.isLoginMode()) {
      const token = btoa(`${this.login}:${this.password}`);

      this.taskService.login(token).subscribe({
        next: () => {
          localStorage.setItem('auth_token', token);
          this.taskService.currentUser.set(this.login);
          this.router.navigate(['/app']);
        },
        error: () => {
          alert('Неверный логин или пароль');
        }
      });

    } else {
      alert('Регистрация пока не доступна. Используйте логин: admin, пароль: admin');
      this.isLoginMode.set(true);
    }
  }
}
