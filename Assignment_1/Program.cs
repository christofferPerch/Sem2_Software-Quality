using Assignment_1.Data;
using Assignment_1.Services;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Choose the connection string based on environment (use TaskManager_TestDB for tests)
var environment = builder.Environment.EnvironmentName;
var connectionString = environment == "Test"
    ? builder.Configuration.GetConnectionString("TestDatabase") // Use the TestDatabase connection string in test environments
    : builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configure the application database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register the data access and services
builder.Services.AddScoped<IDataAccess, SqlDataAccess>(sp =>
    new SqlDataAccess(connectionString));

builder.Services.AddScoped<ToDoService>();
builder.Services.AddScoped<CategoryService>();

// Add developer tools and Identity
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Enable HSTS for production
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Set up the default routing for MVC controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ToDo}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
