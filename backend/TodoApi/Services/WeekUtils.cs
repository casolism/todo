namespace TodoApi.Services;

public static class WeekUtils
{
    /// <summary>Lunes ISO de la semana a la que pertenece la fecha dada.</summary>
    public static DateOnly GetWeekStart(DateOnly date)
    {
        var diff = ((int)date.DayOfWeek + 6) % 7; // Monday = 0 ... Sunday = 6
        return date.AddDays(-diff);
    }
}
