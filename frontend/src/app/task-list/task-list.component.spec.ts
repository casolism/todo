import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { TaskListComponent } from './task-list.component';
import { TaskService } from '../task.service';
import { Task } from '../models/task';

describe('TaskListComponent', () => {
  let fixture: ComponentFixture<TaskListComponent>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let tasks: Task[];

  beforeEach(async () => {
    tasks = [
      { id: 1, description: 'Tarea pendiente', completed: false },
      { id: 2, description: 'Tarea terminada', completed: true }
    ];

    taskServiceSpy = jasmine.createSpyObj('TaskService', ['getTasks', 'toggleTask']);
    taskServiceSpy.getTasks.and.returnValue(of(tasks));

    await TestBed.configureTestingModule({
      imports: [TaskListComponent],
      providers: [{ provide: TaskService, useValue: taskServiceSpy }]
    }).compileComponents();

    fixture = TestBed.createComponent(TaskListComponent);
    fixture.detectChanges();
  });

  it('should load tasks from the service on init', () => {
    expect(taskServiceSpy.getTasks).toHaveBeenCalled();
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
    taskServiceSpy.toggleTask.and.returnValue(of({ id: 1, description: 'Tarea pendiente', completed: true }));

    const checkbox: HTMLInputElement = fixture.nativeElement.querySelector('input[type="checkbox"]');
    checkbox.checked = true;
    checkbox.dispatchEvent(new Event('change'));
    fixture.detectChanges();

    expect(taskServiceSpy.toggleTask).toHaveBeenCalledWith(1);

    const li = fixture.nativeElement.querySelector('li');
    expect(li.classList).toContain('completed');
    expect(checkbox.checked).toBeTrue();
  });
});
