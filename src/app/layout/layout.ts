import { Component, inject, signal, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { CalendarComponent } from '../calendar/calendar';
import { TaskListComponent } from '../tasks/task-list/task-list';
import { ThemeService } from '../services/theme';
import { TaskService } from '../services/task';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, CalendarComponent, TaskListComponent, DatePipe],
  templateUrl: './layout.html',
  styleUrl: './layout.css',
})
export class LayoutComponent implements OnInit, OnDestroy {
  themeService = inject(ThemeService);
  taskService = inject(TaskService);
  router = inject(Router);

  now = signal(new Date());
  isUserMenuOpen = signal(false);
  private timer: any;

  ngOnInit() {
    this.timer = setInterval(() => {
      this.now.set(new Date());
      this.checkNotifications();
    }, 1000);
  }

  checkNotifications() {
    const tasks = this.taskService.tasks();
    const now = this.now();

    tasks.forEach(task => {
      if (task.completed) return;
      if (!task.startTime) return;

      const [hours, minutes] = task.startTime.split(':').map(Number);
      const taskDate = new Date(task.date);
      taskDate.setHours(hours, minutes, 0, 0);

      // Check if task is today
      if (taskDate.getDate() !== now.getDate() ||
        taskDate.getMonth() !== now.getMonth() ||
        taskDate.getFullYear() !== now.getFullYear()) {
        return;
      }

      const diff = taskDate.getTime() - now.getTime();
      // 1 hour = 60 * 60 * 1000 = 3600000 ms
      // Notify if within 60 mins (e.g. 59:59 to 60:00)
      // To avoid multiple alerts, we might need a flag "notified" in local state, 
      // but for this simple lab, we can just log or show a toast if we had one.
      // Or checking if diff is exactly roughly 1 hour (e.g. between 59m 50s and 60m).

      if (diff > 3590000 && diff < 3600000) {
        // Simple browser notification or alert
        // Using Notification API if available, else alert
        if (typeof Notification !== 'undefined' && Notification.permission === 'granted') {
          new Notification('Напоминание', { body: `Скоро задача: ${task.title}` });
        } else if (typeof Notification !== 'undefined' && Notification.permission !== 'denied') {
          Notification.requestPermission().then(permission => {
            if (permission === 'granted') {
              new Notification('Напоминание', { body: `Скоро задача: ${task.title}` });
            }
          });
        } else {
          console.log(`Напоминание: ${task.title} через час!`);
        }
      }
    });
  }

  ngOnDestroy() {
    if (this.timer) {
      clearInterval(this.timer);
    }
  }

  toggleTheme() {
    this.themeService.toggleTheme();
  }

  toggleUserMenu() {
    this.isUserMenuOpen.update(v => !v);
  }

  logout() {
    if (typeof localStorage !== 'undefined') {
      localStorage.removeItem('auth_token');
    }
    this.router.navigate(['/login']);
  }
}
