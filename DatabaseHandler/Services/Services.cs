using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseHandler.Repository;
using LiteDB;
using Microsoft.Extensions.Logging;
using WorldGeneration.Models;

namespace DatabaseHandler.Services
{
    public class Services<T> : IServices<T>
    {
  //      private readonly ILogger<Services<T>> _log;
        private readonly IRepository<T> _repository;

        public Services(IRepository<T> repository)
        {
            _repository = repository;
          //  _log = log;
        }

        public Task<BsonValue> InsertAsync(T obj)
        {
            try
            {
                return _repository.InsertAsync(obj);
            }
            catch (Exception ex)
            {
             //   _log.LogError("Exception: {Message}", ex.Message);
                throw;
            }
        }

        public Task<T> ReadAsync(T obj)
        {
            try
            {
                return _repository.ReadAsync(obj);
            }
            catch (Exception ex)
            {
         //       _log.LogError("Exception: {Message}", ex.Message);
                throw;
            }
        }

        public Task<int> UpdateAsync(T obj)
        {
            try
            {
                return _repository.UpdateAsync(obj);
            }
            catch (Exception ex)
            {
      //          _log.LogError("Exception: {Message}", ex.Message);
                throw;
            }
        }

        public Task<int> DeleteAsync(T obj)
        {
            try
            {
                return _repository.DeleteAsync(obj);
            }
            catch (Exception ex)
            {
 //               _log.LogError("Exception: {Message}", ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<int> DeleteAllAsync()
        {
            return _repository.DeleteAllAsync();
        }
    }
}