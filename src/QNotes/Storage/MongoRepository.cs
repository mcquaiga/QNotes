using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using MongoDB.Driver;
using QNotes.Core.Models;
using System.Threading.Tasks;

public class MongoRepository<TEntity>
    where TEntity : BaseEntity
{
    private IMongoClient _client;
    private IMongoDatabase _db;

    public MongoRepository()
    {
        _client = new MongoClient("mongodb://localhost:27017");
        _db = _client.GetDatabase("q-notes");     
    }

    public async Task Save(TEntity entity)
    {   
        await Collection.InsertOneAsync(entity);
    }

    public async Task<TEntity> Get(Guid? id)
    {
        return await Collection.Find(x => x.Id == id).SingleAsync();
    }

    public async Task Delete(Guid id, TEntity entity)
    {
        await Collection.DeleteOneAsync(x => x.Id == id);
    }

    public async Task Update(Guid id, TEntity entity)
    {
        await Collection.ReplaceOneAsync(x => x.Id == id, entity);
    }

    public async Task<IEnumerable<TEntity>> FindAll()
    {
        return await Collection.Find("{}").ToListAsync();
    }

    private IMongoCollection<TEntity> Collection
    {
        get
        {
            return _db.GetCollection<TEntity>(typeof(TEntity).Name);
        }
    }
}