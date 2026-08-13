using Core.Contexts;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories
{
    public class ProvinceRepository : IProvinceRepository
    {
        private readonly AppDbContext _context;

        public ProvinceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Province province)
        {
            await _context.Provinces.AddAsync(province);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Province>> GetAllAsync() =>
            await _context.Provinces
                .AsNoTracking()
                .OrderBy(province => province.Name)
                .ToListAsync();

        public Task<Province?> GetByNameAsync(string name) =>
            _context.Provinces.FirstOrDefaultAsync(
                province => province.Name.ToLower() == name.ToLower());
    }
}
