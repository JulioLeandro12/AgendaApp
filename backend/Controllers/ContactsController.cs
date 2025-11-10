using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactApi.Data;
using ContactApi.Models;

namespace ContactApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ContactsController(AppDbContext db) { _db = db; }

        // GET: api/Contacts -> Get all contacts
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Contacts.OrderBy(c => c.Name).ToListAsync();
            return Ok(list);
        }

        // GET: api/Contacts/{id} -> Get a specific contact by ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var c = await _db.Contacts.FindAsync(id);
            if (c == null) return NotFound();
            return Ok(c);
        }

        // POST: api/Contacts -> Create a new contact
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Contact contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _db.Contacts.Add(contact);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = contact.Id }, contact);
        }

        // PUT: api/Contacts/{id} -> Update an existing contact
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Contact contact)
        {
            if (id != contact.Id) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState); // Invalid model state

            _db.Entry(contact).State = EntityState.Modified;
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _db.Contacts.AnyAsync(c => c.Id == id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Contacts/{id} -> Delete a contact
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _db.Contacts.FindAsync(id);
            if (c == null) return NotFound();
            _db.Contacts.Remove(c);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
