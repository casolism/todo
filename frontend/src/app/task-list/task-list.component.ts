import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Task } from '../models/task';
import { TaskService } from '../task.service';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css'
})
export class TaskListComponent implements OnInit {
  tasks: Task[] = [];

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.taskService.getTasks().subscribe((tasks) => (this.tasks = tasks));
  }

  onToggle(task: Task): void {
    this.taskService.toggleTask(task.id).subscribe((updated) => {
      task.completed = updated.completed;
    });
  }

  onAdd(description: string, input: HTMLInputElement): void {
    const trimmed = description.trim();
    if (!trimmed) return;

    this.taskService.addTask(trimmed).subscribe((task) => {
      this.tasks.push(task);
      input.value = '';
    });
  }
}
