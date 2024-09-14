using Assignment_2.Models;
using Assignment_2.Services;
using DataAccess;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace Assignment_2_Tests.ToDoServiceIntegrationTests
{
    public class ToDoServiceIntegrationTests : IDisposable
    {
        private readonly ToDoService _toDoService;
        private readonly CategoryService _categoryService;

        public ToDoServiceIntegrationTests()
        {
            var serviceProvider = TestStartup.InitializeServices();
            _toDoService = serviceProvider.GetRequiredService<ToDoService>();
            _categoryService = serviceProvider.GetRequiredService<CategoryService>();
        }

        [Fact]
        public async Task InsertAndGetToDo_ShouldReturnInsertedToDo()
        {
            var newCategory = new Category { CategoryName = "Test Category" };
            var categoryId = await _categoryService.InsertCategory(newCategory);

            Assert.True(categoryId > 0, "Category insert failed");

            var newToDo = new ToDo
            {
                Title = "Test ToDo",
                Description = "Description for test",
                Deadline = DateTime.UtcNow.AddDays(1),
                IsCompleted = false,
                CategoryId = categoryId
            };

            var todoId = await _toDoService.InsertToDo(newToDo);
            var insertedToDo = await _toDoService.GetToDoById(todoId);

            Assert.NotNull(insertedToDo);
            Assert.Equal(newToDo.Title, insertedToDo.Title);
            Assert.Equal(newToDo.Description, insertedToDo.Description);
            Assert.Equal(newToDo.CategoryId, insertedToDo.CategoryId);
        }

        [Fact]
        public async Task GetAllToDos_ShouldReturnMultipleToDos()
        {
            var newCategory = new Category { CategoryName = "Multiple ToDos Category" };
            var categoryId = await _categoryService.InsertCategory(newCategory);

            var todos = new List<ToDo>
            {
                new ToDo { Title = "ToDo 1", Description = "Description 1", Deadline = DateTime.UtcNow.AddDays(1), IsCompleted = false, CategoryId = categoryId },
                new ToDo { Title = "ToDo 2", Description = "Description 2", Deadline = DateTime.UtcNow.AddDays(2), IsCompleted = false, CategoryId = categoryId },
                new ToDo { Title = "ToDo 3", Description = "Description 3", Deadline = DateTime.UtcNow.AddDays(3), IsCompleted = false, CategoryId = categoryId }
            };

            foreach (var todo in todos)
            {
                await _toDoService.InsertToDo(todo);
            }

            var allToDos = await _toDoService.GetAllToDos();

            Assert.True(allToDos.Count >= 3);
            Assert.Contains(allToDos, t => t.Title == "ToDo 1");
            Assert.Contains(allToDos, t => t.Title == "ToDo 2");
            Assert.Contains(allToDos, t => t.Title == "ToDo 3");
        }

        [Fact]
        public async Task UpdateToDo_ShouldModifyToDoInDatabase()
        {
            var newCategory = new Category { CategoryName = "Update Category" };
            var categoryId = await _categoryService.InsertCategory(newCategory);

            var newToDo = new ToDo
            {
                Title = "Original ToDo",
                Description = "Original Description",
                Deadline = DateTime.UtcNow.AddDays(1),
                IsCompleted = false,
                CategoryId = categoryId
            };

            var todoId = await _toDoService.InsertToDo(newToDo);

            var updatedToDo = new ToDo
            {
                Id = todoId,
                Title = "Updated ToDo",
                Description = "Updated Description",
                Deadline = DateTime.UtcNow.AddDays(2),
                IsCompleted = true,
                CategoryId = categoryId
            };

            await _toDoService.UpdateToDo(updatedToDo);
            var result = await _toDoService.GetToDoById(todoId);

            Assert.NotNull(result);
            Assert.Equal("Updated ToDo", result.Title);
            Assert.Equal("Updated Description", result.Description);
            Assert.Equal(true, result.IsCompleted);
        }

        [Fact]
        public async Task DeleteToDo_ShouldRemoveToDoFromDatabase()
        {
            var newCategory = new Category { CategoryName = "Delete Category" };
            var categoryId = await _categoryService.InsertCategory(newCategory);

            var newToDo = new ToDo
            {
                Title = "ToDo to delete",
                Description = "Description for deletion",
                Deadline = DateTime.UtcNow.AddDays(1),
                IsCompleted = false,
                CategoryId = categoryId
            };

            var todoId = await _toDoService.InsertToDo(newToDo);
            await _toDoService.DeleteToDo(todoId);
            var deletedToDo = await _toDoService.GetToDoById(todoId);

            Assert.Null(deletedToDo);
        }

        [Fact]
        public async Task InsertToDo_WithInvalidCategoryId_ShouldThrowSqlException()
        {
            var invalidToDo = new ToDo
            {
                Title = "Invalid ToDo",
                Description = "Description for invalid ToDo",
                Deadline = DateTime.UtcNow.AddDays(1),
                IsCompleted = false,
                CategoryId = -1
            };

            var exception = await Assert.ThrowsAsync<Exception>(async () => await _toDoService.InsertToDo(invalidToDo));

            Assert.IsType<SqlException>(exception.InnerException);
        }

        [Fact]
        public async Task InsertAndDeleteCategory_ShouldWorkCorrectly()
        {
            var newCategory = new Category { CategoryName = "Category to Insert and Delete" };
            var categoryId = await _categoryService.InsertCategory(newCategory);

            Assert.True(categoryId > 0);

            await _categoryService.DeleteCategory(categoryId);
            var deletedCategory = await _categoryService.GetCategoryById(categoryId);

            Assert.Null(deletedCategory);
        }

        public void Dispose()
        {
            CleanupDatabase();
        }

        private void CleanupDatabase()
        {
            var serviceProvider = TestStartup.InitializeServices();
            var dataAccess = serviceProvider.GetRequiredService<IDataAccess>();

            var deleteSql = "DELETE FROM ToDo; DELETE FROM Category;";
            var resetIdentitySql = @"
                DBCC CHECKIDENT('ToDo', RESEED, 0);
                DBCC CHECKIDENT('Category', RESEED, 0);";

            dataAccess.Delete(deleteSql, new { }).Wait();
            dataAccess.Delete(resetIdentitySql, new { }).Wait();
        }
    }
}
