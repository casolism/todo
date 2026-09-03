const MONTHS_ES = [
  'ene', 'feb', 'mar', 'abr', 'may', 'jun', 'jul', 'ago', 'sep', 'oct', 'nov', 'dic'
];

/** Lunes de la semana ISO a la que pertenece `date`. */
export function mondayOf(date: Date): Date {
  const result = new Date(date.getFullYear(), date.getMonth(), date.getDate());
  const dayNum = (result.getDay() + 6) % 7; // Monday = 0 ... Sunday = 6
  result.setDate(result.getDate() - dayNum);
  return result;
}

export function addWeeks(date: Date, weeks: number): Date {
  const result = new Date(date);
  result.setDate(result.getDate() + weeks * 7);
  return result;
}

/** Formato YYYY-MM-DD (el que espera el backend en `week=`). */
export function toIsoDate(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}

/** Número de semana ISO-8601 (1-53). */
export function isoWeekNumber(date: Date): number {
  const d = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
  const dayNum = (d.getUTCDay() + 6) % 7;
  d.setUTCDate(d.getUTCDate() - dayNum + 3);
  const firstThursday = new Date(Date.UTC(d.getUTCFullYear(), 0, 4));
  const firstThursdayDayNum = (firstThursday.getUTCDay() + 6) % 7;
  firstThursday.setUTCDate(firstThursday.getUTCDate() - firstThursdayDayNum + 3);
  return 1 + Math.round((d.getTime() - firstThursday.getTime()) / (7 * 24 * 3600 * 1000));
}

/** Ej. "7-13 sep" o "29 sep - 5 oct" si la semana cruza de mes. */
export function formatWeekRange(weekStart: Date): string {
  const weekEnd = new Date(weekStart);
  weekEnd.setDate(weekEnd.getDate() + 6);

  const startMonth = MONTHS_ES[weekStart.getMonth()];
  const endMonth = MONTHS_ES[weekEnd.getMonth()];

  if (startMonth === endMonth) {
    return `${weekStart.getDate()}-${weekEnd.getDate()} ${startMonth}`;
  }
  return `${weekStart.getDate()} ${startMonth} - ${weekEnd.getDate()} ${endMonth}`;
}
