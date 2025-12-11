import { Component, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskService } from '../services/task';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './calendar.html',
  styleUrl: './calendar.css',
})
export class CalendarComponent {
  taskService = inject(TaskService);

  currentDate = signal(new Date());
  selectedDate = signal(new Date());

  months = [
    'Янв', 'Фев', 'Мар', 'Апр', 'Май', 'Июн',
    'Июл', 'Авг', 'Сен', 'Окт', 'Ноя', 'Дек'
  ];

  weekDays = ['ПН', 'ВТ', 'СР', 'ЧТ', 'ПТ', 'СБ', 'ВС'];

  currentYear = computed(() => this.currentDate().getFullYear());
  currentMonthIndex = computed(() => this.currentDate().getMonth());

  calendarDays = computed(() => {
    const year = this.currentYear();
    const month = this.currentMonthIndex();
    const firstDayOfMonth = new Date(year, month, 1);
    const lastDayOfMonth = new Date(year, month + 1, 0);

    const days = [];

    // Fill previous month days
    let startDay = firstDayOfMonth.getDay(); // 0 (Sun) to 6 (Sat)
    // Adjust for Monday start (Monday=1, Sunday=7)
    startDay = startDay === 0 ? 6 : startDay - 1;

    for (let i = 0; i < startDay; i++) {
      days.push({ day: null, date: null, tasks: [] });
    }

    // Fill current month days
    for (let i = 1; i <= lastDayOfMonth.getDate(); i++) {
      const date = new Date(year, month, i);
      const dateString = date.toISOString().split('T')[0];
      const tasks = this.taskService.tasks().filter(t => t.date === dateString);
      days.push({ day: i, date: date, tasks });
    }

    return days;
  });

  selectMonth(index: number) {
    const newDate = new Date(this.currentDate());
    newDate.setMonth(index);
    this.currentDate.set(newDate);
  }

  changeYear(delta: number) {
    const newDate = new Date(this.currentDate());
    newDate.setFullYear(newDate.getFullYear() + delta);
    this.currentDate.set(newDate);
  }

  selectDate(day: any) {
    if (!day.date) return;
    this.selectedDate.set(day.date);
    this.taskService.selectedDate.set(day.date);
  }

  isSelected(day: any): boolean {
    if (!day.date) return false;
    const sel = this.selectedDate();
    return day.date.getDate() === sel.getDate() &&
      day.date.getMonth() === sel.getMonth() &&
      day.date.getFullYear() === sel.getFullYear();
  }
}
