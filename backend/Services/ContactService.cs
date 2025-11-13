using AutoMapper;
using ContactApi.DTOs;
using ContactApi.Models;
using ContactApi.Repositories;

namespace ContactApi.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _repository;
        private readonly IMapper _mapper;

        public ContactService(IContactRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ContactDto>> GetAllAsync()
        {
            var contacts = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ContactDto>>(contacts);
        }

        public async Task<ContactDto?> GetByIdAsync(int id)
        {
            var contact = await _repository.GetByIdAsync(id);
            return _mapper.Map<ContactDto?>(contact);
        }

        public async Task<ContactDto> CreateAsync(ContactDto dto)
        {
            if (await _repository.ExistsByEmailAsync(dto.Email))
                throw new InvalidOperationException("Já existe um contato com esse e-mail.");

            var contact = _mapper.Map<Contact>(dto);
            var created = await _repository.AddAsync(contact);
            return _mapper.Map<ContactDto>(created);
        }

        public async Task UpdateAsync(ContactDto dto)
        {
            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing == null)
                throw new KeyNotFoundException("Contato não encontrado.");

            _mapper.Map(dto, existing);
            await _repository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
