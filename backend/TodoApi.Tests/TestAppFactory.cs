using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TodoApi.Tests;

/// <summary>
/// WebApplicationFactory que apunta TaskStore a un archivo JSON temporal por
/// instancia, para que los tests no lean/escriban el tasks.json real de
/// desarrollo ni se pisen entre sí.
/// </summary>
public class TestAppFactory : WebApplicationFactory<Program>
{
    private readonly bool _ownsFile;

    public string FilePath { get; }

    public TestAppFactory()
        : this(Path.Combine(Path.GetTempPath(), $"tasks-test-{Guid.NewGuid():N}.json"), ownsFile: true)
    {
    }

    internal TestAppFactory(string filePath, bool ownsFile = false)
    {
        FilePath = filePath;
        _ownsFile = ownsFile;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["TasksFilePath"] = FilePath
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (_ownsFile && File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }
    }
}
