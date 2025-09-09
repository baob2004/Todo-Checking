using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<TodoItem> TodoItems => Set<TodoItem>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TodoItem>().HasData(
                new TodoItem { Id = 1, Title = "Learn EF Core", IsDone = false, Due = new DateTime(2025, 09, 15) },
                new TodoItem { Id = 2, Title = "Setup Swagger UI", IsDone = true, Due = new DateTime(2025, 09, 10) },
                new TodoItem { Id = 3, Title = "Write API docs", IsDone = false, Due = new DateTime(2025, 09, 20) },
                new TodoItem { Id = 4, Title = "Finish Clean Architecture tutorial", IsDone = false, Due = new DateTime(2025, 09, 18) },
                new TodoItem { Id = 5, Title = "Implement JWT Auth", IsDone = false, Due = new DateTime(2025, 09, 25) },
                new TodoItem { Id = 6, Title = "Create Todo CRUD", IsDone = true, Due = new DateTime(2025, 09, 05) },
                new TodoItem { Id = 7, Title = "Write Unit Tests", IsDone = false, Due = new DateTime(2025, 09, 28) },
                new TodoItem { Id = 8, Title = "Configure CORS", IsDone = true, Due = new DateTime(2025, 09, 07) },
                new TodoItem { Id = 9, Title = "Add Logging with NLog", IsDone = false, Due = new DateTime(2025, 09, 30) },
                new TodoItem { Id = 10, Title = "Deploy API to Azure", IsDone = false, Due = new DateTime(2025, 10, 05) }
            );
        }
    }
}