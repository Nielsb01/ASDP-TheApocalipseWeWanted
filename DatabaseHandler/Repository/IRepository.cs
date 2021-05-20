using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseHandler.Poco;
using LiteDB;

namespace DatabaseHandler.Repository
{
    public interface IRepository<T>
    {
        Task<BsonValue> CreateAsync(T obj);
        Task<T> ReadAsync(T obj);
        Task<int> UpdateAsync(T obj);
        Task<int> DeleteAsync(T obj);
        Task<IEnumerable<T>> GetAllAsync();
        Task<int> DeleteAllAsync();
        public Task<IEnumerable<PlayerPoco>> GetAllPoco();

    }
}