using Core.Models;

namespace Core.Interfaces
{
    public interface IProvinceRepository
    {
        Task<bool> AddAsync(Province province);
        Task<IEnumerable<Province>> GetAllAsync();
        Task<Province?> GetByNameAsync(string name);
    }
}
