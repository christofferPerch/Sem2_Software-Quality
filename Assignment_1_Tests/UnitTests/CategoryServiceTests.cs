using Assignment_1.Models;
using Assignment_1.Services;
using DataAccess;
using Moq;

namespace Assignment_1_Tests.UnitTests
{
    public class CategoryServiceTests
    {
        private readonly Mock<IDataAccess> _mockDataAccess;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _mockDataAccess = new Mock<IDataAccess>();
            _categoryService = new CategoryService(_mockDataAccess.Object);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsCategory_WhenCategoryExists()
        {
            var categoryId = 1;
            var expectedCategory = new Category { Id = categoryId, CategoryName = "Test Category" };
            _mockDataAccess.Setup(x => x.GetById<Category>(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync(expectedCategory);

            var result = await _categoryService.GetCategoryById(categoryId);

            Assert.NotNull(result);
            Assert.Equal(expectedCategory.Id, result.Id);
            Assert.Equal(expectedCategory.CategoryName, result.CategoryName);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsNull_WhenCategoryDoesNotExist()
        {
            _mockDataAccess.Setup(x => x.GetById<Category>(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync((Category?)null);

            var result = await _categoryService.GetCategoryById(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllCategories_ReturnsListOfCategories()
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, CategoryName = "Category 1" },
                new Category { Id = 2, CategoryName = "Category 2" }
            };
            _mockDataAccess.Setup(x => x.GetAll<Category>(It.IsAny<string>(), It.IsAny<object?>()))
                           .ReturnsAsync(categories);

            var result = await _categoryService.GetAllCategories();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Category 1", result[0].CategoryName);
            Assert.Equal("Category 2", result[1].CategoryName);
        }

        [Fact]
        public async Task InsertCategory_ReturnsInsertedId()
        {
            var newCategory = new Category { CategoryName = "New Category" };
            _mockDataAccess.Setup(x => x.Insert(It.IsAny<string>(), It.IsAny<Category>()))
                           .ReturnsAsync(1);

            var result = await _categoryService.InsertCategory(newCategory);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task UpdateCategory_ExecutesUpdateMethod()
        {
            var existingCategory = new Category { Id = 1, CategoryName = "Updated Category" };

            _mockDataAccess.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<Category>()))
                           .ReturnsAsync(1);

            await _categoryService.UpdateCategory(existingCategory);

            _mockDataAccess.Verify(x => x.Update(It.IsAny<string>(), It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCategory_ExecutesDeleteMethod()
        {
            var categoryId = 1;
            _mockDataAccess.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync(1);

            await _categoryService.DeleteCategory(categoryId);

            _mockDataAccess.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }
    }
}
