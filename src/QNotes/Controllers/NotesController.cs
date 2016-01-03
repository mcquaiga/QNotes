using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using QNotes.API.Models;
using QNotes.API.Data.Services;
using Microsoft.AspNet.Authorization;

namespace QNotes.Controllers
{
    [Route("api/[controller]")]
    public class NotesController : Controller
    {
        IEntityService<Note> _noteService { get; set; }

        public NotesController(IEntityService<Note> noteService)
        {
            _noteService = noteService;
        }

        // GET: api/notes
        [HttpGet]
        public async Task<IEnumerable<Note>> Get()
        {
            return await _noteService.GetAllAsync();
        }

        // GET api/notes/{guid}
        [HttpGet("{id}"), Authorize]
        public async Task<Note> Get(string id)
        {
            return await _noteService.GetByIdAsync(id);
        }

        // POST api/notes
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Note note)
        {
            if (note == null) return HttpBadRequest("Note object was empty or was an invalid model.");

            await _noteService.CreateAsync(note);
            return new ObjectResult(note);
        }

        // PUT api/notes/9627e454-1633-4485-9911-4cd5fea4b339
        //[HttpPut("{id}")]
        //public async void Put(Guid id, [FromBody]Note note)
        //{
        //    await GetRepo().Update(id, note);
        //}

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async void Delete(string id, [FromBody]Note note)
        {
            await _noteService.DeleteAsync(id);
        }
    }
}
