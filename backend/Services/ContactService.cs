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
            if (await _repository.ExistsByEmailAsync(dto.Email))
                throw new InvalidOperationException("Já existe um contato com esse e-mail.");

            var contact = FromDto(dto);
            var created = await _repository.AddAsync(contact);
            return ToDto(created);
        }

        public async Task UpdateAsync(ContactDto dto)
        {
            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing == null)
                throw new KeyNotFoundException("Contato não encontrado.");

            // update properties manually
            existing.Name = dto.Name;
            existing.Email = dto.Email;
            existing.Phone = dto.Phone;
            await _repository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        // Mapping helpers (private)
        private static ContactDto ToDto(Contact c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            Phone = c.Phone
        };

        private static Contact FromDto(ContactDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone
        };
    }
}
