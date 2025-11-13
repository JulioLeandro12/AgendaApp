// using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// using ContactApi.Data;
// using ContactApi.Models;

// namespace ContactApi.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class ContactsController : ControllerBase
//     {
//         private readonly AppDbContext _db;
//         public ContactsController(AppDbContext db) { _db = db; }

//         // GET: api/Contacts -> Get all contacts
//         [HttpGet]
//         public async Task<IActionResult> GetAll()
//         {
//             var list = await _db.Contacts.OrderBy(c => c.Name).ToListAsync();
//             return Ok(list);
//         }

//         // GET: api/Contacts/{id} -> Get a specific contact by ID
//         [HttpGet("{id:int}")]
//         public async Task<IActionResult> Get(int id)
//         {
//             var c = await _db.Contacts.FindAsync(id);
//             if (c == null) return NotFound(new { message = "Contato não encontrado." });
//             return Ok(c);
//         }

//         // POST: api/Contacts -> Create a new contact
//         // [HttpPost]
//         // public async Task<IActionResult> Create([FromBody] Contact contact)
//         // {
//         //     if (!ModelState.IsValid) return BadRequest(ModelState);
//         //     _db.Contacts.Add(contact);
//         //     await _db.SaveChangesAsync();
//         //     return CreatedAtAction(nameof(Get), new { id = contact.Id }, contact);
//         // }`

//         // POST: api/Contacts -> Create a new contact
//         [HttpPost]
//         public async Task<ActionResult<Contact>> Create(Contact contact)
//         {
//             if (!ModelState.IsValid)
//                 return BadRequest(ModelState);

//             try
//             {
//                 await _db.Contacts.AddAsync(contact);
//                 await _db.SaveChangesAsync();
//                 return CreatedAtAction(nameof(Get), new { id = contact.Id }, contact);
//             }
//             catch (DbUpdateException ex)
//             {
//                 return StatusCode(500, new { message = "Erro ao salvar no banco de dados.", details = ex.Message });
//             }
//         }

//         // PUT: api/Contacts/{id} -> Update an existing contact
//         // [HttpPut("{id:int}")]
//         // public async Task<IActionResult> Update(int id, [FromBody] Contact contact)
//         // {
//         //     if (id != contact.Id) return BadRequest();
//         //     if (!ModelState.IsValid) return BadRequest(ModelState); // Invalid model state

//         //     _db.Entry(contact).State = EntityState.Modified;
//         //     try
//         //     {
//         //         await _db.SaveChangesAsync();
//         //     }
//         //     catch (DbUpdateConcurrencyException)
//         //     {
//         //         if (!await _db.Contacts.AnyAsync(c => c.Id == id)) return NotFound();
//         //         throw;
//         //     }

//         //     return NoContent();
//         // }

//         // PUT: api/Contacts/{id} -> Update an existing contact
//         [HttpPut("{id}")]
//         public async Task<IActionResult> Update(int id, Contact contact)
//         {
//             if (id != contact.Id)
//                 return BadRequest(new { message = "O ID informado não confere com o contato ou não existe." });

//             if (!ModelState.IsValid)
//                 return BadRequest(ModelState);

//             _db.Entry(contact).State = EntityState.Modified;

//             try
//             {
//                 await _db.SaveChangesAsync();
//                 return NoContent();
//             }
//             catch (DbUpdateConcurrencyException)
//             {
//                 if (!_db.Contacts.Any(e => e.Id == id))
//                     return NotFound(new { message = "Contato não encontrado para atualização." });

//                 throw;
//             }
//         }

//         // DELETE: api/Contacts/{id} -> Delete a contact
//         // [HttpDelete("{id:int}")]
//         // public async Task<IActionResult> Delete(int id)
//         // {
//         //     var c = await _db.Contacts.FindAsync(id);
//         //     if (c == null) return NotFound();
//         //     _db.Contacts.Remove(c);
//         //     await _db.SaveChangesAsync();
//         //     return NoContent();
//         // }


//         // DELETE: api/Contacts/{id} -> Delete a contact
//          [HttpDelete("{id}")]
//         public async Task<IActionResult> Delete(int id)
//         {
//             var contact = await _db.Contacts.FindAsync(id);
//             if (contact == null)
//                 return NotFound(new { message = "Contato não encontrado para exclusão." });

//             _db.Contacts.Remove(contact);
//             await _db.SaveChangesAsync();

//             return Ok(new { message = "Contato removido com sucesso." });
//         }
//     }
// }


using ContactApi.DTOs;
using ContactApi.Services;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;


namespace ContactApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _service;

        public ContactController(IContactService service)
        {
            _service = service;
        }

        // GET: api/Contacts -> Get all contacts
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        // GET: api/Contacts/{id} -> Get a specific contact by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contact = await _service.GetByIdAsync(id);
            return contact == null ? NotFound() : Ok(contact);
        }

        // POST: api/Contacts -> Create a new contact
        [HttpPost]
        public async Task<IActionResult> Create(ContactDto dto)
        {
            if (!ModelState.IsValid)
            {
                // log ModelState errors to console to check what's populando
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState errors: " + string.Join(" | ", errors));
            }


            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/Contacts/{id} -> Update an existing contact
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ContactDto dto)
        {
            if (id != dto.Id) return BadRequest("ID inconsistente");
            await _service.UpdateAsync(dto);
            return NoContent();
        }

        // DELETE: api/Contacts/{id} -> Delete a contact
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        // Debug endpoint to check if the validator is registered
        [HttpGet("debug-validator")]
        public IActionResult DebugValidator([FromServices] IValidator<ContactDto> validator)
        {
            return validator == null ? NotFound("Validator not registered") : Ok("Validator registered: " + validator.GetType().FullName);
        }

    }
}
