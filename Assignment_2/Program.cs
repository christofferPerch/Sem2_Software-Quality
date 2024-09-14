namespace Assignment_2.Program
{
    using Assignment_2.Services;
    using DataAccess;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Configure Database Access
            var environment = builder.Environment.EnvironmentName;
            var connectionString = environment == "Test"
                ? builder.Configuration.GetConnectionString("TestDatabase") // Use the TestDatabase connection string in test environments
                : builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddScoped<IDataAccess, SqlDataAccess>(sp =>
                new SqlDataAccess(connectionString));

            // Register your services
            builder.Services.AddScoped<ToDoService>();
            builder.Services.AddScoped<CategoryService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
