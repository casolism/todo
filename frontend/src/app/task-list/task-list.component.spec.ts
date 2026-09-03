import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { TaskListComponent } from './task-list.component';
import { TaskService } from '../task.service';
import { Task } from '../models/task';
import { mondayOf, toIsoDate } from '../date-utils';

describe('TaskListComponent', () => {
  let fixture: ComponentFixture<TaskListComponent>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let tasks: Task[];
  const thisWeek = toIsoDate(mondayOf(new Date()));

  beforeEach(async () => {
    tasks = [
      { id: 1, description: 'Tarea pendiente', completed: false, priority: 'media', weekStart: thisWeek },
      { id: 2, description: 'Tarea terminada', completed: true, priority: 'alta', weekStart: thisWeek }
    ];

    taskServiceSpy = jasmine.createSpyObj('TaskService', [
      'getTasks', 'toggleTask', 'addTask', 'deleteTask', 'carryOver'
    ]);
    taskServiceSpy.getTasks.and.returnValue(of(tasks));

    await TestBed.configureTestingModule({
      imports: [TaskListComponent],
      providers: [{ provide: TaskService, useValue: taskServiceSpy }]
    }).compileComponents();

    fixture = TestBed.createComponent(TaskListComponent);
    fixture.detectChanges();
  });

  it('should load tasks from the service on init', () => {
    expect(taskServiceSpy.getTasks).toHaveBeenCalledWith(thisWeek);
    expect(fixture.componentInstance.tasks.length).toBe(2);
  });

  it('should render one checkbox per task reflecting its completed state', () => {
    const checkboxes: NodeListOf<HTMLInputElement> =
      fixture.nativeElement.querySelectorAll('input[type="checkbox"]');

    expect(checkboxes.length).toBe(2);
    expect(checkboxes[0].checked).toBeFalse();
    expect(checkboxes[1].checked).toBeTrue();
  });

  it('should call PUT (toggleTask) and refresh the view when a checkbox is clicked', () => {
    taskServiceSpy.toggleTask.and.returnValue(
      of({ id: 1, description: 'Tarea pendiente', completed: true, priority: 'media', weekStart: thisWeek })
    );

    const checkbox: HTMLInputElement = fixture.nativeElement.querySelector('input[type="checkbox"]');
    checkbox.checked = true;
    checkbox.dispatchEvent(new Event('change'));
    fixture.detectChanges();

    expect(taskServiceSpy.toggleTask).toHaveBeenCalledWith(1);

    const li = fixture.nativeElement.querySelector('li');
    expect(li.classList).toContain('completed');
    expect(checkbox.checked).toBeTrue();
  });

  it('should call POST (addTask) and add the new task to the list on submit', () => {
    taskServiceSpy.addTask.and.returnValue(
      of({ id: 3, description: 'Regar las plantas', completed: false, priority: 'media', weekStart: thisWeek })
    );

    const input: HTMLInputElement = fixture.nativeElement.querySelector('input[type="text"]');
    const form: HTMLFormElement = fixture.nativeElement.querySelector('form');
    input.value = 'Regar las plantas';
    input.dispatchEvent(new Event('input'));
    form.dispatchEvent(new Event('submit'));
    fixture.detectChanges();

    expect(taskServiceSpy.addTask).toHaveBeenCalledWith('Regar las plantas', 'media', thisWeek);
    expect(fixture.componentInstance.tasks.length).toBe(3);

    const items = fixture.nativeElement.querySelectorAll('li');
    expect(items.length).toBe(3);
    expect(input.value).toBe('');
  });

  it('should call DELETE (deleteTask) and remove the task from the list on delete click', () => {
    taskServiceSpy.deleteTask.and.returnValue(of(undefined));

    const deleteButton: HTMLButtonElement = fixture.nativeElement.querySelector('.delete-btn');
    deleteButton.click();
    fixture.detectChanges();

    expect(taskServiceSpy.deleteTask).toHaveBeenCalledWith(1);
    expect(fixture.componentInstance.tasks.length).toBe(1);

    const items = fixture.nativeElement.querySelectorAll('li');
    expect(items.length).toBe(1);
  });

  it('should call GET with the next week when the "next" nav button is clicked', () => {
    const nextWeekIso = toIsoDate(new Date(mondayOf(new Date()).getTime() + 7 * 24 * 3600 * 1000));

    const nextButton: HTMLButtonElement = fixture.nativeElement.querySelector('.nav-btn[aria-label="Semana siguiente"]');
    nextButton.click();
    fixture.detectChanges();

    expect(taskServiceSpy.getTasks).toHaveBeenCalledWith(nextWeekIso);
  });

  it('should call GET with the previous week when the "prev" nav button is clicked', () => {
    const prevWeekIso = toIsoDate(new Date(mondayOf(new Date()).getTime() - 7 * 24 * 3600 * 1000));

    const prevButton: HTMLButtonElement = fixture.nativeElement.querySelector('.nav-btn[aria-label="Semana anterior"]');
    prevButton.click();
    fixture.detectChanges();

    expect(taskServiceSpy.getTasks).toHaveBeenCalledWith(prevWeekIso);
  });

  it('should show the "Semana actual" badge only for the current week, and hide it after navigating away', () => {
    let badge = fixture.nativeElement.querySelector('.current-badge');
    expect(badge).not.toBeNull();

    const nextButton: HTMLButtonElement = fixture.nativeElement.querySelector('.nav-btn[aria-label="Semana siguiente"]');
    nextButton.click();
    fixture.detectChanges();

    badge = fixture.nativeElement.querySelector('.current-badge');
    expect(badge).toBeNull();
  });

  it('should go back to the current week when "Ir a hoy" is clicked', () => {
    const nextButton: HTMLButtonElement = fixture.nativeElement.querySelector('.nav-btn[aria-label="Semana siguiente"]');
    nextButton.click();
    fixture.detectChanges();

    const todayButton: HTMLButtonElement = fixture.nativeElement.querySelector('.today-btn');
    todayButton.click();
    fixture.detectChanges();

    expect(taskServiceSpy.getTasks).toHaveBeenCalledWith(thisWeek);
    expect(fixture.nativeElement.querySelector('.current-badge')).not.toBeNull();
  });
});
