namespace TodoApi.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public string Priority { get; set; } = TaskPriority.Media;
    public DateOnly WeekStart { get; set; }
}

public static class TaskPriority
{
    public const string Alta = "alta";
    public const string Media = "media";
    public const string Baja = "baja";

    public static readonly IReadOnlySet<string> Valid = new HashSet<string> { Alta, Media, Baja };
}
