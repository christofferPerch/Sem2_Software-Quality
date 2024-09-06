﻿namespace DataAccess
{
    public interface IDataAccess
    {
        Task<T?> GetById<T>(string sql, object parameters);
        Task<List<T>> GetAll<T>(string sql, object? parameters = null);
        Task<int> Insert(string sql, object parameters);
        Task<int> Update(string sql, object parameters);
        Task<int> Delete(string sql, object parameters);

    }
}
