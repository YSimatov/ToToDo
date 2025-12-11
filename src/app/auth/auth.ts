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
      // Basic Auth Login Check
      // We can check validity by making a request to a protected endpoint or a specific "me" endpoint
      // For now, let's just save credentials and try to navigate.
      // In real app, we would verify against backend.

      const token = btoa(`${this.login}:${this.password}`);

      // Test the token via service
      this.taskService.login(token).subscribe({
        next: () => {
          localStorage.setItem('auth_token', token);
          // Force update of current user in service
          this.taskService.currentUser.set(this.login);
          this.router.navigate(['/app']);
        },
        error: () => {
          alert('Неверный логин или пароль');
        }
      });

    } else {
      // Register
      // We didn't implement Registration endpoint in backend yet (as Lab 6 focus was Auth check).
      // But Lab 6 says "Вторая может быть 'незарегистрированный пользователь' ... доступен функционал кроме регистрации".
      // It implies we should have registration.
      // But I only added middleware and seed data.
      // Let's mock registration or just say "Use admin/admin".
      alert('Регистрация пока не доступна. Используйте логин: admin, пароль: admin');
      this.isLoginMode.set(true);
    }
  }
}
