export type Priority = 'alta' | 'media' | 'baja';

export interface Task {
  id: number;
  description: string;
  completed: boolean;
  priority: Priority;
  weekStart: string; // YYYY-MM-DD
}
