using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class ApplicationDbContext : DbContext {
    public DbSet<ToDoItem> ToDoItems { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
    }
}
