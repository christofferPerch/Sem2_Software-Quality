using Assignment_2.Models;
using DataAccess;

namespace Assignment_2.Services
{
    public class CategoryService
    {
        private readonly IDataAccess _dataAccess;

        public CategoryService(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            var sql = @"SELECT * FROM Category WHERE Id = @Id";
            var parameters = new { Id = id };
            return await _dataAccess.GetById<Category>(sql, parameters);
        }

        public async Task<List<Category>> GetAllCategories()
        {
            var sql = "SELECT * FROM Category";
            return await _dataAccess.GetAll<Category>(sql);
        }

        public async Task<int> InsertCategory(Category category)
        {
            var sql = @"INSERT INTO Category (CategoryName) OUTPUT INSERTED.Id VALUES (@CategoryName)";
            return await _dataAccess.Insert(sql, category);
        }


        public async Task UpdateCategory(Category category)
        {
            var sql = @"UPDATE Category SET CategoryName = @CategoryName WHERE Id = @Id";
            await _dataAccess.Update(sql, category);
        }

        public async Task DeleteCategory(int id)
        {
            var sql = "DELETE FROM Category WHERE Id = @Id";
            var parameters = new { Id = id };
            await _dataAccess.Delete(sql, parameters);
        }
    }
}
