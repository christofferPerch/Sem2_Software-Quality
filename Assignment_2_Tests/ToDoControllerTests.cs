using Assignment_2.Controllers;
using Assignment_2.Models;
using Assignment_2.Services;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Assignment_2_Tests
{
    public class ToDoControllerTests
    {
        private readonly Mock<IDataAccess> _mockDataAccess;
        private readonly ToDoService _toDoService;
        private readonly ToDoController _toDoController;

        public ToDoControllerTests()
        {
            _mockDataAccess = new Mock<IDataAccess>();
            _toDoService = new ToDoService(_mockDataAccess.Object);
            _toDoController = new ToDoController(_toDoService);
        }

        [Fact]
        public async Task GetAllToDos_ShouldReturnOkResult_WithListOfToDos()
        {
            var todos = new List<ToDo>
            {
                new ToDo { Id = 1, Title = "Test ToDo 1", Description = "Description 1", Deadline = DateTime.UtcNow.AddDays(1), IsCompleted = false, CategoryId = 1 },
                new ToDo { Id = 2, Title = "Test ToDo 2", Description = "Description 2", Deadline = DateTime.UtcNow.AddDays(2), IsCompleted = false, CategoryId = 1 }
            };
            _mockDataAccess.Setup(da => da.GetAll<ToDo>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(todos);

            var result = await _toDoController.GetAllToDos();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTodos = Assert.IsType<List<ToDo>>(okResult.Value);
            Assert.Equal(2, returnedTodos.Count);
        }

        [Fact]
        public async Task GetToDoById_ShouldReturnOkResult_WithToDo()
        {
            var todo = new ToDo { Id = 1, Title = "Test ToDo", Description = "Description", Deadline = DateTime.UtcNow.AddDays(1), IsCompleted = false, CategoryId = 1 };
            _mockDataAccess.Setup(da => da.GetById<ToDo>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(todo);

            var result = await _toDoController.GetToDoById(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedToDo = Assert.IsType<ToDo>(okResult.Value);
            Assert.Equal(1, returnedToDo.Id);
        }

        [Fact]
        public async Task GetToDoById_ShouldReturnNotFound_WhenToDoDoesNotExist()
        {
            _mockDataAccess.Setup(da => da.GetById<ToDo>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync((ToDo)null);

            var result = await _toDoController.GetToDoById(1);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateToDo_ShouldReturnCreatedAtAction_WithNewToDoId()
        {
            var newToDo = new ToDo { Title = "New ToDo", Description = "Description", Deadline = DateTime.UtcNow.AddDays(1), IsCompleted = false, CategoryId = 1 };
            _mockDataAccess.Setup(da => da.Insert(It.IsAny<string>(), It.IsAny<ToDo>())).ReturnsAsync(1);

            var result = await _toDoController.CreateToDo(newToDo);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_toDoController.GetToDoById), createdAtActionResult.ActionName);
            Assert.Equal(1, createdAtActionResult.RouteValues["id"]);
        }

        [Fact]
        public async Task UpdateToDo_ShouldReturnNoContent_WhenToDoIsUpdated()
        {
            var existingToDo = new ToDo { Id = 1, Title = "Existing ToDo", Description = "Description", Deadline = DateTime.UtcNow.AddDays(1), IsCompleted = false, CategoryId = 1 };
            _mockDataAccess.Setup(da => da.GetById<ToDo>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(existingToDo);

            var updatedToDo = new ToDo { Id = 1, Title = "Updated ToDo", Description = "Updated Description", Deadline = DateTime.UtcNow.AddDays(2), IsCompleted = true, CategoryId = 1 };
            var result = await _toDoController.UpdateToDo(1, updatedToDo);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateToDo_ShouldReturnBadRequest_WhenIdDoesNotMatch()
        {
            var todo = new ToDo { Id = 1, Title = "Test ToDo", Description = "Description", Deadline = DateTime.UtcNow.AddDays(1), IsCompleted = false, CategoryId = 1 };

            var result = await _toDoController.UpdateToDo(2, todo);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateToDo_ShouldReturnNotFound_WhenToDoDoesNotExist()
        {
            var updatedToDo = new ToDo { Id = 1, Title = "Updated ToDo", Description = "Updated Description", Deadline = DateTime.UtcNow.AddDays(2), IsCompleted = true, CategoryId = 1 };
            _mockDataAccess.Setup(da => da.GetById<ToDo>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync((ToDo)null);

            var result = await _toDoController.UpdateToDo(1, updatedToDo);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteToDo_ShouldReturnNoContent_WhenToDoIsDeleted()
        {
            var existingToDo = new ToDo { Id = 1, Title = "Existing ToDo", Description = "Description", Deadline = DateTime.UtcNow.AddDays(1), IsCompleted = false, CategoryId = 1 };
            _mockDataAccess.Setup(da => da.GetById<ToDo>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(existingToDo);

            var result = await _toDoController.DeleteToDo(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteToDo_ShouldReturnNotFound_WhenToDoDoesNotExist()
        {
            _mockDataAccess.Setup(da => da.GetById<ToDo>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync((ToDo)null);

            var result = await _toDoController.DeleteToDo(1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
