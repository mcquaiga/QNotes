using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using QNotes.Core.Models;

namespace QNotes.Controllers
{
    [Route("api/[controller]")]
    public class NotesController : Controller
    {
        // GET: api/notes
        [HttpGet]
        public Task<IEnumerable<Note>> Get()
        {
            return GetRepo().FindAll();
        }

        // GET api/notes/{guid}
        [HttpGet("{id}")]
        public async Task<Note> Get(Guid id)
        {
            return await GetRepo().Get(id);
        }

        // POST api/notes
        [HttpPost]
        public async void Post([FromBody]Note note)
        {
            await GetRepo().Save(note);
        }

        // PUT api/notes/9627e454-1633-4485-9911-4cd5fea4b339
        [HttpPut("{id}")]
        public async void Put(Guid id, [FromBody]Note note)
        {
            await GetRepo().Update(id, note);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async void Delete(Guid id, [FromBody]Note note)
        {
            await GetRepo().Delete(id, note);
        }

        private MongoRepository<Note> GetRepo()
        {
            return new MongoRepository<Note>();
        }
    }
}
