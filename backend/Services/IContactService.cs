using ContactApi.DTOs;

namespace ContactApi.Services
{
    public interface IContactService
    {
        Task<IEnumerable<ContactDto>> GetAllAsync();
        Task<ContactDto?> GetByIdAsync(int id);
        Task<ContactDto> CreateAsync(ContactDto dto);
        Task UpdateAsync(ContactDto dto);
        Task DeleteAsync(int id);
    }
}
