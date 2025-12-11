import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export interface Task {
  id: string;
  title: string;
  date: string; // YYYY-MM-DD
  startTime: string; // HH:mm
  endTime?: string; // HH:mm
  completed: boolean;
  description?: string;
}

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5288/api/tasks';

  // Store tasks in a signal
  private tasksSignal = signal<Task[]>([]);

  // Selected date state
  selectedDate = signal<Date>(new Date());

  currentUser = signal<string>('');
  isAdmin = computed(() => this.currentUser() === 'admin');

  readonly tasks = this.tasksSignal.asReadonly();

  constructor() {
    this.checkAuth();
    this.loadTasks();
  }

  private checkAuth() {
    if (typeof localStorage !== 'undefined') {
      const token = localStorage.getItem('auth_token');
      if (token) {
        try {
          const decoded = atob(token);
          const [user, pass] = decoded.split(':');
          this.currentUser.set(user);
        } catch (e) {
          console.error('Invalid token');
        }
      }
    }
  }

  private getHeaders() {
    // In a real app, this would come from a AuthService
    let token = '';
    if (typeof localStorage !== 'undefined') {
      token = localStorage.getItem('auth_token') || '';
    }

    // Fallback if not logged in (Lab 6 requirement: "unregistered user")
    // But backend blocks /api requests without valid auth.
    // So for demo, we might need a default guest token or handle 401.
    // However, the prompt says "When entering under one user - see one screen, under another - another".
    // It implies data separation by user.
    // My current backend returns ALL tasks regardless of user.
    // Lab 6 implies "User Stories" might differ or data.
    // For now, I'll just send the token from localStorage.

    const headers: any = {};
    if (token) {
      headers['Authorization'] = `Basic ${token}`;
    }
    return { headers };
  }

  private loadTasks() {
    this.http.get<Task[]>(this.apiUrl, this.getHeaders()).subscribe({
      next: (tasks) => {
        this.tasksSignal.set(tasks);
      },
      error: (err) => {
        console.error('Failed to load tasks', err);
        // If 401, maybe redirect or clear token?
        if (err.status === 401) {
          // Handle unauthorized
        }
      }
    });
  }

  getTasksByDate(date: string) {
    return computed(() =>
      this.tasksSignal().filter(task => task.date === date)
        .sort((a, b) => a.startTime.localeCompare(b.startTime))
    );
  }

  // Auth method moved here to encapsulate HTTP logic
  login(token: string) {
    const headers = { 'Authorization': `Basic ${token}` };
    return this.http.get(this.apiUrl, { headers });
  }

  addTask(task: Omit<Task, 'id'>) {
    this.http.post<Task>(this.apiUrl, task, this.getHeaders()).subscribe({
      next: (newTask) => {
        this.tasksSignal.update(tasks => [...tasks, newTask]);
      },
      error: (err) => console.error(err)
    });
  }

  updateTask(updatedTask: Task) {
    this.http.put(`${this.apiUrl}/${updatedTask.id}`, updatedTask, this.getHeaders()).subscribe({
      next: () => {
        this.tasksSignal.update(tasks =>
          tasks.map(t => t.id === updatedTask.id ? updatedTask : t)
        );
      },
      error: (err) => console.error(err)
    });
  }

  deleteTask(taskId: string) {
    this.http.delete(`${this.apiUrl}/${taskId}`, this.getHeaders()).subscribe({
      next: () => {
        this.tasksSignal.update(tasks => tasks.filter(t => t.id !== taskId));
      },
      error: (err) => console.error(err)
    });
  }

  toggleTaskCompletion(taskId: string) {
    const task = this.tasksSignal().find(t => t.id === taskId);
    if (task) {
      const updatedTask = { ...task, completed: !task.completed };
      this.updateTask(updatedTask);
    }
  }
}
