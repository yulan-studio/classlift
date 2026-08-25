using Core.Interfaces;
using Core.Models;

namespace Core.Services
{
    public class ProvinceService : IProvinceService
    {
        private readonly IProvinceRepository _provinceRepository;

        public ProvinceService(IProvinceRepository provinceRepository)
        {
            _provinceRepository = provinceRepository;
        }

        public async Task<bool> AddAsync(Province province)
        {
            province.Name = province.Name.Trim();
            if (string.IsNullOrWhiteSpace(province.Name))
                throw new ArgumentException("Province name cannot be empty.");

            if (await _provinceRepository.GetByNameAsync(province.Name) != null)
                throw new InvalidOperationException("This province already exists.");

            return await _provinceRepository.AddAsync(province);
        }

        public Task<IEnumerable<Province>> GetAllAsync() =>
            _provinceRepository.GetAllAsync();
    }
}
