using Assignment_1.Models;
using Assignment_1.Services;
using DataAccess;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace Assignment_1_Tests
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

            var insertedCategory = await _categoryService.GetCategoryById(categoryId);

            Console.WriteLine($"Retrieved Category: {insertedCategory}");

            Assert.NotNull(insertedCategory);
            Assert.Equal(newCategory.CategoryName, insertedCategory.CategoryName);

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
            Dispose();
        }


        [Fact]
        public async Task DeleteToDo_ShouldNotBeRetrievable()
        {
            var newCategory = new Category { CategoryName = "Delete Category" };
            var categoryId = await _categoryService.InsertCategory(newCategory);

            var newToDo = new ToDo
            {
                Title = "ToDo to be deleted",
                Description = "Description",
                Deadline = DateTime.UtcNow.AddDays(1),
                IsCompleted = false,
                CategoryId = categoryId
            };

            var todoId = await _toDoService.InsertToDo(newToDo);

            await _toDoService.DeleteToDo(todoId);
            var deletedToDo = await _toDoService.GetToDoById(todoId);

            Assert.Null(deletedToDo);
            Dispose();
        }

        [Fact]
        public async Task InsertMultipleToDos_ShouldReturnAllToDos()
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

            Assert.Equal(3, allToDos.Count);
            Assert.Contains(allToDos, t => t.Title == "ToDo 1");
            Assert.Contains(allToDos, t => t.Title == "ToDo 2");
            Assert.Contains(allToDos, t => t.Title == "ToDo 3");
            Dispose();
        }

        [Fact]
        public async Task InsertToDo_WithNonExistentCategory_ShouldThrowSqlException()
        {
            var newToDo = new ToDo
            {
                Title = "ToDo with invalid CategoryId",
                Description = "Description",
                Deadline = DateTime.UtcNow.AddDays(1),
                IsCompleted = false,
                CategoryId = -1
            };

            var exception = await Assert.ThrowsAsync<Exception>(async () => await _toDoService.InsertToDo(newToDo));

            Assert.IsType<SqlException>(exception.InnerException);
            Dispose();
        }


        [Fact]
        public async Task UpdateCategory_ShouldModifyCategoryInDatabase()
        {
            var category = new Category { CategoryName = "Old Category" };
            var categoryId = await _categoryService.InsertCategory(category);

            var updatedCategory = new Category
            {
                Id = categoryId,
                CategoryName = "Updated Category"
            };

            await _categoryService.UpdateCategory(updatedCategory);
            var result = await _categoryService.GetCategoryById(categoryId);

            Assert.NotNull(result);
            Assert.Equal("Updated Category", result.CategoryName);
        }

        [Fact]
        public async Task DeleteCategory_ShouldRemoveCategoryFromDatabase()
        {
            var category = new Category { CategoryName = "Category to Delete" };
            var categoryId = await _categoryService.InsertCategory(category);

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
