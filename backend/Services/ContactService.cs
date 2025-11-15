using ContactApi.DTOs;
using ContactApi.Models;
using ContactApi.Repositories;

namespace ContactApi.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _repository;
        
        public ContactService(IContactRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ContactDto>> GetAllAsync()
        {
            var contacts = await _repository.GetAllAsync();
            return contacts.Select(ToDto);
        }

        public async Task<ContactDto?> GetByIdAsync(int id)
        {
            var contact = await _repository.GetByIdAsync(id);
            return contact == null ? null : ToDto(contact);
        }

        public async Task<ContactDto> CreateAsync(ContactDto dto)
        {
            if (await _repository.ExistsByNameAsync(dto.Name))
                throw new InvalidOperationException("Nome do contato já existe.");
            if (await _repository.ExistsByEmailAsync(dto.Email))
                throw new InvalidOperationException("Este email já existe.");

            var contact = FromDto(dto);
            // Ensure Id is not set on create
            contact.Id = 0;
            var created = await _repository.AddAsync(contact);
            return ToDto(created);
        }

        public async Task UpdateAsync(ContactDto dto)
        {
            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing == null)
                throw new KeyNotFoundException("Contato não encontrado.");

            if (!string.Equals(existing.Name, dto.Name, StringComparison.Ordinal))
            {
                if (await _repository.ExistsByNameAsync(dto.Name, dto.Id))
                    throw new InvalidOperationException("Nome do contato já existe.");
            }

            if (!string.Equals(existing.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await _repository.ExistsByEmailAsync(dto.Email, dto.Id))
                    throw new InvalidOperationException("Este email já existe.");
            }

            existing.Name = dto.Name;
            existing.Email = dto.Email;
            existing.Phone = dto.Phone;
            await _repository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        private static ContactDto ToDto(Contact contact)
        {
            return new ContactDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Phone = contact.Phone
            };
        }

        private static Contact FromDto(ContactDto dto)
        {
            return new Contact
            {
                Id = dto.Id,
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone
            };
        }
    }
}
