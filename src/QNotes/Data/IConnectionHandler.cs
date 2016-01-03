using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using QNotes.API.Models;

public interface IConnectionHandler<TEntity> where TEntity : IMongoEntity
{
    Task Delete(string id);
    Task<IAsyncCursor<TEntity>> Find(FilterDefinition<TEntity> filter);
    Task<IEnumerable<TEntity>> FindAll();
    Task<TEntity> Get(string id);
    Task Save(TEntity entity);
    Task Update(ObjectId id, TEntity entity);
}