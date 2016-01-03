using MongoDB.Driver;
using QNotes.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QNotes.API.Data.Services
{
    public class NoteService : EntityService<Note>
    {
        public NoteService(IConnectionHandler<Note> connectionHandler) : base(connectionHandler) { }

        public async override Task UpdateAsync(Note entity)
        {
            throw new NotImplementedException("Note service update not implemented.");
        }
    }
}
