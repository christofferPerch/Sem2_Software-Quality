using Assignment_1.Models;
using Assignment_1.Services;
using DataAccess;
using Moq;

namespace Assignment_1_Tests.UnitTests
{
    public class ToDoServiceTests
    {
        private readonly Mock<IDataAccess> _mockDataAccess;
        private readonly ToDoService _toDoService;

        public ToDoServiceTests()
        {
            _mockDataAccess = new Mock<IDataAccess>();
            _toDoService = new ToDoService(_mockDataAccess.Object);
        }

        [Fact]
        public async Task GetToDoById_ReturnsToDo_WhenToDoExists()
        {
            var todoId = 1;
            var expectedToDo = new ToDo { Id = todoId, Title = "Test ToDo", CategoryId = 1 };
            _mockDataAccess.Setup(x => x.GetById<ToDo>(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync(expectedToDo);
            _mockDataAccess.Setup(x => x.GetById<Category>(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync(new Category { Id = 1, CategoryName = "Test Category" });

            var result = await _toDoService.GetToDoById(todoId);

            Assert.NotNull(result);
            Assert.Equal(expectedToDo.Id, result.Id);
            Assert.Equal(expectedToDo.Title, result.Title);
            Assert.NotNull(result.Category);
        }

        [Fact]
        public async Task GetToDoById_ReturnsNull_WhenToDoDoesNotExist()
        {
            _mockDataAccess.Setup(x => x.GetById<ToDo>(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync((ToDo?)null);

            var result = await _toDoService.GetToDoById(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllToDos_ReturnsListOfToDos()
        {
            var todos = new List<ToDo>
            {
                new ToDo { Id = 1, Title = "ToDo 1", CategoryId = 1 },
                new ToDo { Id = 2, Title = "ToDo 2", CategoryId = 2 }
            };
            _mockDataAccess.Setup(x => x.GetAll<ToDo>(It.IsAny<string>(), It.IsAny<object?>()))
                           .ReturnsAsync(todos);
            _mockDataAccess.Setup(x => x.GetById<Category>(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync(new Category { Id = 1, CategoryName = "Test Category" });

            var result = await _toDoService.GetAllToDos();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task InsertToDo_ReturnsInsertedId()
        {
            var newToDo = new ToDo { Title = "New ToDo", CategoryId = 1 };
            _mockDataAccess.Setup(x => x.Insert(It.IsAny<string>(), It.IsAny<ToDo>()))
                           .ReturnsAsync(1);

            var result = await _toDoService.InsertToDo(newToDo);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task UpdateToDo_ExecutesUpdateMethod()
        {
            var existingToDo = new ToDo { Id = 1, Title = "Updated ToDo", CategoryId = 1 };

            _mockDataAccess.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<ToDo>()))
                           .ReturnsAsync(1);

            await _toDoService.UpdateToDo(existingToDo);

            _mockDataAccess.Verify(x => x.Update(It.IsAny<string>(), It.IsAny<ToDo>()), Times.Once);
        }

        [Fact]
        public async Task DeleteToDo_ExecutesDeleteMethod()
        {
            var todoId = 1;
            _mockDataAccess.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync(1);

            await _toDoService.DeleteToDo(todoId);

            _mockDataAccess.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }
    }
}
