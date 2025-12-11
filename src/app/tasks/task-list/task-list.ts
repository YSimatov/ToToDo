import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskService, Task } from '../../services/task';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './task-list.html',
  styleUrl: './task-list.css',
})
export class TaskListComponent {
  taskService = inject(TaskService);

  isFormVisible = signal(false);
  editingTask = signal<Task | null>(null);

  // Form model
  newTaskTitle = '';
  newTaskTimeStart = '';
  newTaskTimeEnd = '';
  newTaskDescription = '';

  selectedDateTasks = computed(() => {
    const selectedDateStr = this.taskService.selectedDate().toISOString().split('T')[0];
    return this.taskService.tasks()
      .filter(t => t.date === selectedDateStr)
      .sort((a, b) => a.startTime.localeCompare(b.startTime));
  });

  openAddForm() {
    this.resetForm();
    this.isFormVisible.set(true);
    this.editingTask.set(null);
  }

  closeForm() {
    this.isFormVisible.set(false);
    this.resetForm();
  }

  saveTask() {
    if (!this.newTaskTitle || !this.newTaskTimeStart) return;

    const taskData = {
      title: this.newTaskTitle,
      date: this.taskService.selectedDate().toISOString().split('T')[0],
      startTime: this.newTaskTimeStart,
      endTime: this.newTaskTimeEnd,
      completed: false,
      description: this.newTaskDescription
    };

    if (this.editingTask()) {
      this.taskService.updateTask({
        ...this.editingTask()!,
        ...taskData,
        id: this.editingTask()!.id,
        completed: this.editingTask()!.completed
      });
    } else {
      this.taskService.addTask(taskData);
    }

    this.closeForm();
  }

  editTask(task: Task) {
    this.editingTask.set(task);
    this.newTaskTitle = task.title;
    this.newTaskTimeStart = task.startTime;
    this.newTaskTimeEnd = task.endTime || '';
    this.newTaskDescription = task.description || '';
    this.isFormVisible.set(true);
  }

  deleteTask(taskId: string) {
    if (confirm('Удалить задачу?')) {
      this.taskService.deleteTask(taskId);
    }
  }

  toggleComplete(task: Task) {
    this.taskService.toggleTaskCompletion(task.id);
  }

  private resetForm() {
    this.newTaskTitle = '';
    this.newTaskTimeStart = '';
    this.newTaskTimeEnd = '';
    this.newTaskDescription = '';
  }
}
