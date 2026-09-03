import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Task } from '../models/task';
import { TaskService } from '../task.service';
import { addWeeks, formatWeekRange, isoWeekNumber, mondayOf, toIsoDate } from '../date-utils';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css'
})
export class TaskListComponent implements OnInit {
  tasks: Task[] = [];
  currentWeekStart: Date = mondayOf(new Date());

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  get currentWeekIso(): string {
    return toIsoDate(this.currentWeekStart);
  }

  get weekNumber(): number {
    return isoWeekNumber(this.currentWeekStart);
  }

  get weekRangeLabel(): string {
    return formatWeekRange(this.currentWeekStart);
  }

  get isCurrentWeek(): boolean {
    return this.currentWeekIso === toIsoDate(mondayOf(new Date()));
  }

  loadTasks(): void {
    this.taskService.getTasks(this.currentWeekIso).subscribe((tasks) => (this.tasks = tasks));
  }

  goToPreviousWeek(): void {
    this.currentWeekStart = addWeeks(this.currentWeekStart, -1);
    this.loadTasks();
  }

  goToNextWeek(): void {
    this.currentWeekStart = addWeeks(this.currentWeekStart, 1);
    this.loadTasks();
  }

  goToToday(): void {
    this.currentWeekStart = mondayOf(new Date());
    this.loadTasks();
  }

  onToggle(task: Task): void {
    this.taskService.toggleTask(task.id).subscribe((updated) => {
      task.completed = updated.completed;
    });
  }

  onAdd(description: string, input: HTMLInputElement): void {
    const trimmed = description.trim();
    if (!trimmed) return;

    this.taskService.addTask(trimmed, 'media', this.currentWeekIso).subscribe((task) => {
      this.tasks.push(task);
      input.value = '';
    });
  }

  onDelete(task: Task): void {
    this.taskService.deleteTask(task.id).subscribe(() => {
      this.tasks = this.tasks.filter((t) => t.id !== task.id);
    });
  }
}
