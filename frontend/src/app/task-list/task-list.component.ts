import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { Priority, Task } from '../models/task';
import { TaskService } from '../task.service';
import { addWeeks, formatWeekRange, isoWeekNumber, mondayOf, toIsoDate } from '../date-utils';

export type TaskFilter = 'todas' | 'pendientes' | 'completadas';

const PRIORITY_LABELS: Record<Priority, string> = {
  alta: 'Alta',
  media: 'Media',
  baja: 'Baja'
};

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css'
})
export class TaskListComponent implements OnInit {
  tasks: Task[] = [];
  currentWeekStart: Date = mondayOf(new Date());
  activeFilter: TaskFilter = 'todas';
  readonly filters: TaskFilter[] = ['todas', 'pendientes', 'completadas'];

  readonly priorities: Priority[] = ['alta', 'media', 'baja'];
  showAddModal = false;
  newTaskDescription = '';
  newTaskPriority: Priority = 'media';
  taskPendingDelete: Task | null = null;

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

  get totalCount(): number {
    return this.tasks.length;
  }

  get doneCount(): number {
    return this.tasks.filter((t) => t.completed).length;
  }

  get progressPercent(): number {
    return this.totalCount === 0 ? 0 : Math.round((this.doneCount / this.totalCount) * 100);
  }

  get visibleTasks(): Task[] {
    switch (this.activeFilter) {
      case 'pendientes':
        return this.tasks.filter((t) => !t.completed);
      case 'completadas':
        return this.tasks.filter((t) => t.completed);
      default:
        return this.tasks;
    }
  }

  setFilter(filter: TaskFilter): void {
    this.activeFilter = filter;
  }

  filterLabel(filter: TaskFilter): string {
    return { todas: 'Todas', pendientes: 'Pendientes', completadas: 'Completadas' }[filter];
  }

  priorityLabel(priority: Priority): string {
    return PRIORITY_LABELS[priority];
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

  openAddModal(): void {
    this.newTaskDescription = '';
    this.newTaskPriority = 'media';
    this.showAddModal = true;
  }

  closeAddModal(): void {
    this.showAddModal = false;
  }

  confirmAdd(): void {
    const trimmed = this.newTaskDescription.trim();
    if (!trimmed) return;

    this.taskService.addTask(trimmed, this.newTaskPriority, this.currentWeekIso).subscribe((task) => {
      this.tasks.push(task);
      this.showAddModal = false;
    });
  }

  onDeleteClick(task: Task): void {
    this.taskPendingDelete = task;
  }

  cancelDelete(): void {
    this.taskPendingDelete = null;
  }

  confirmDelete(): void {
    const task = this.taskPendingDelete;
    if (!task) return;

    this.taskService.deleteTask(task.id).subscribe(() => {
      this.tasks = this.tasks.filter((t) => t.id !== task.id);
      this.taskPendingDelete = null;
    });
  }

  onCarryOver(): void {
    this.taskService.carryOver(this.currentWeekIso).subscribe(() => {
      this.loadTasks();
    });
  }
}
