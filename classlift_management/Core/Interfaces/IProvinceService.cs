using Core.Models;

namespace Core.Interfaces
{
    public interface IProvinceService
    {
        Task<bool> AddAsync(Province province);
        Task<IEnumerable<Province>> GetAllAsync();
    }
}
