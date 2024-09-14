using Assignment_2.Models;
using DataAccess;

namespace Assignment_2.Services
{
    public class ToDoService
    {
        private readonly IDataAccess _dataAccess;

        public ToDoService(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public async Task<ToDo?> GetToDoById(int id)
        {
            var sql = @"
                            SELECT t.Id, t.Title, t.Description, t.Deadline, t.IsCompleted, 
                            t.CategoryId
                            FROM ToDo t
                            WHERE t.Id = @Id";

            var toDo = await _dataAccess.GetById<ToDo>(sql, new { Id = id });

            if (toDo != null)
            {
                toDo.Category = await _dataAccess.GetById<Category>("SELECT * FROM Category WHERE Id = @Id", new { Id = toDo.CategoryId });
            }

            return toDo;
        }

        public async Task<List<ToDo>> GetAllToDos()
        {
            var sql = @"
                            SELECT t.Id, t.Title, t.Description, t.Deadline, t.IsCompleted, 
                            c.Id as CategoryId, c.CategoryName
                            FROM ToDo t
                            LEFT JOIN Category c ON t.CategoryId = c.Id";

            var todos = await _dataAccess.GetAll<ToDo>(sql);

            foreach (var todo in todos)
            {
                todo.Category = await _dataAccess.GetById<Category>("SELECT * FROM Category WHERE Id = @Id", new { Id = todo.CategoryId });
            }

            return todos;
        }

        public async Task<int> InsertToDo(ToDo toDo)
        {
            toDo.IsCompleted = false;
            var sql = @"
                INSERT INTO ToDo (Title, Description, Deadline, IsCompleted, CategoryId)
                VALUES (@Title, @Description, @Deadline, @IsCompleted, @CategoryId)";
            return await _dataAccess.Insert(sql, toDo);
        }

        public async Task UpdateToDo(ToDo toDo)
        {
            var sql = @"
                UPDATE ToDo
                SET Title = @Title, Description = @Description, Deadline = @Deadline, 
                    IsCompleted = @IsCompleted, CategoryId = @CategoryId
                WHERE Id = @Id";
            await _dataAccess.Update(sql, toDo);
        }

        public async Task DeleteToDo(int id)
        {
            var sql = "DELETE FROM ToDo WHERE Id = @Id";
            var parameters = new { Id = id };
            await _dataAccess.Delete(sql, parameters);
        }
    }
}
