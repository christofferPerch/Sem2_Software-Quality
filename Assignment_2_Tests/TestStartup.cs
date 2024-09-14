using Assignment_2.Services;
using DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class TestStartup
{
    public static ServiceProvider InitializeServices()
    {
        var basePath = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(basePath, "appsettings.Test.json");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile(configPath, optional: false, reloadOnChange: true)
            .Build();


        var services = new ServiceCollection();

        var connectionString = configuration.GetConnectionString("TestDatabase");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'TestDatabase' not found.");
        }

        services.AddScoped<IDataAccess, SqlDataAccess>(sp =>
            new SqlDataAccess(connectionString));

        services.AddScoped<ToDoService>();
        services.AddScoped<CategoryService>();

        return services.BuildServiceProvider();
    }
}
