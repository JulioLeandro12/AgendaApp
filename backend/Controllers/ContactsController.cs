using ContactApi.DTOs;
using ContactApi.Services;
using Microsoft.AspNetCore.Mvc;


namespace ContactApi.Controllers
{
    [ApiController]
    [Route("api/contacts")] // padroniza rota em minúsculo e plural
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _service;

        public ContactsController(IContactService service)
        {
            _service = service;
        }

    // GET: api/contacts -> Get all contacts
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

    // GET: api/contacts/{id} -> Get a specific contact by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contact = await _service.GetByIdAsync(id);
            return contact == null ? NotFound() : Ok(contact);
        }

    // POST: api/contacts -> Create a new contact
        [HttpPost]
        public async Task<IActionResult> Create(ContactDto dto)
        {
            if (!ModelState.IsValid)
            {
                // Retorna 400 com detalhes das validações do FluentValidation
                return ValidationProblem(ModelState);
            }


            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

    // PUT: api/contacts/{id} -> Update an existing contact
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ContactDto dto)
        {
            if (id != dto.Id) return BadRequest("ID inconsistente");
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }
            await _service.UpdateAsync(dto);
            return NoContent();
        }

    // DELETE: api/contacts/{id} -> Delete a contact
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

    }
}
