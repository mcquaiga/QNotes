using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using QNotes.API.Models;

namespace QNotes.API.Data.Services
{
    public abstract class EntityService<T> : IEntityService<T> where T : IMongoEntity
    {
        protected readonly IConnectionHandler<T> ConnectionHandler;

        protected EntityService(IConnectionHandler<T> connectionHandler)
        {
            ConnectionHandler = connectionHandler;
        }

        public async virtual Task CreateAsync(T entity)
        {
            await ConnectionHandler.Save(entity);
        }

        public async virtual Task DeleteAsync(string id)
        {
            await ConnectionHandler.Delete(id);
        }

        public async virtual Task<IEnumerable<T>> GetAllAsync()
        {
            return await ConnectionHandler.FindAll();
        }

        public async virtual Task<T> GetByIdAsync(string id)
        {
            return await ConnectionHandler.Get(id);
        }

        public abstract Task UpdateAsync(T entity);
    }
}
