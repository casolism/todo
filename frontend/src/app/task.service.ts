import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Priority, Task } from './models/task';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private readonly baseUrl = 'http://localhost:5038/api/tasks';

  constructor(private http: HttpClient) {}

  getTasks(week?: string): Observable<Task[]> {
    const params = week ? new HttpParams().set('week', week) : undefined;
    return this.http.get<Task[]>(this.baseUrl, { params });
  }

  addTask(description: string, priority: Priority, weekStart: string): Observable<Task> {
    return this.http.post<Task>(this.baseUrl, { description, priority, weekStart });
  }

  toggleTask(id: number): Observable<Task> {
    return this.http.put<Task>(`${this.baseUrl}/${id}`, {});
  }

  deleteTask(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  carryOver(week: string): Observable<{ moved: number }> {
    const params = new HttpParams().set('week', week);
    return this.http.post<{ moved: number }>(`${this.baseUrl}/carry-over`, {}, { params });
  }
}
