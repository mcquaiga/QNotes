using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using MongoDB.Driver;
using System.Threading.Tasks;
using QNotes.API.Models;
using MongoDB.Bson;
using Microsoft.Extensions.OptionsModel;
using QNotes.API.Settings;

public class MongoConnectionHandler<TEntity> : IConnectionHandler<TEntity>
    where TEntity : IMongoEntity
{
    protected static class MongoConnection
    {
        private static Lazy<IMongoClient> LazyConnection(StorageConfig configOptions) => new Lazy<IMongoClient>(() => { return new MongoClient(configOptions.ConnectionString); });

        public static IMongoClient Connection(StorageConfig configOptions)
        {
            return LazyConnection(configOptions).Value;
        }
    }

    const string connectionString = "mongodb://localhost:27017";
    const string databaseName = "q-notes";

    protected IMongoCollection<TEntity> EntityCollection { get; private set; }

    public MongoConnectionHandler(IOptions<StorageConfig> storageConfig)
    {
        var config = storageConfig.Value;
        var db = MongoConnection.Connection(storageConfig.Value).GetDatabase(config.Database);        
        EntityCollection = db.GetCollection<TEntity>(typeof(TEntity).Name.ToLower() + "s");
    }
    
    public async Task Save(TEntity entity)
    {   
        await EntityCollection.InsertOneAsync(entity);
    }

    public async Task<TEntity> Get(string id) 
        => await EntityCollection.Find(x => x.Id == new ObjectId(id)).SingleAsync();

    public async Task Delete(string id)
    {
        await EntityCollection.DeleteOneAsync(x => x.Id == new ObjectId(id));
    }

    public async Task Update(ObjectId id, TEntity entity)
    {
        entity.Modified = DateTimeOffset.UtcNow;
        await EntityCollection.ReplaceOneAsync(x => x.Id == id, entity);
    }

    public async Task<IEnumerable<TEntity>> FindAll()
        => await EntityCollection.Find("{}").ToListAsync();

    public async Task<IAsyncCursor<TEntity>> Find(FilterDefinition<TEntity> filter)
    {
        return await EntityCollection.FindAsync<TEntity>(filter);
    }
}