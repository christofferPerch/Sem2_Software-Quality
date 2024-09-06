using Assignment_1.Services;
using DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class TestStartup
{
    public static ServiceProvider InitializeServices()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())  
            .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)  
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
