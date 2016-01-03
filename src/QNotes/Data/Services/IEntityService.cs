using QNotes.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QNotes.API.Data.Services
{
    public interface IEntityService<T> where T : IMongoEntity
    {
        Task CreateAsync(T entity);

        Task DeleteAsync(string id);

        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(string id);

        Task UpdateAsync(T entity);
    }
}
