using System.Net.Http.Json;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests;

public class PersistenceTests
{
    [Fact]
    public async Task Tasks_PersistAcrossRestart()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"tasks-persistence-{Guid.NewGuid():N}.json");

        try
        {
            await using (var factory1 = new TestAppFactory(filePath))
            {
                var client1 = factory1.CreateClient();
                var response = await client1.PostAsJsonAsync("/api/tasks", new CreateTaskRequest("Sobrevivir el reinicio"));
                response.EnsureSuccessStatusCode();
            }

            // Simula "reiniciar el proceso": una nueva instancia de la app,
            // apuntando al mismo archivo tasks.json.
            await using (var factory2 = new TestAppFactory(filePath))
            {
                var client2 = factory2.CreateClient();
                var tasks = await client2.GetFromJsonAsync<List<TaskItem>>("/api/tasks");

                Assert.Contains(tasks!, t => t.Description == "Sobrevivir el reinicio");
            }
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
