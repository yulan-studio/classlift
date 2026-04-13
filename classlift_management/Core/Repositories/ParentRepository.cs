using Core.Interfaces;
using Core.Models;
using Core.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;




namespace Core.Repositories
{

    public class ParentRepository : IParentRepository
    {
        private readonly AppDbContext _context;

        public ParentRepository(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get all parents
        public async Task<IEnumerable<Parent>> GetAllAsync()
        {
            return await _context.Parents.ToListAsync();
        }

        // ✅ Get a parent by ID
        public async Task<Parent?> GetAsync(int parentId)
        {
            return await _context.Parents.FindAsync(parentId);
        }

        // ✅ Add a new parent
        public async Task<bool> AddAsync(Parent parent)
        {
            await _context.Parents.AddAsync(parent);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> AddAndReturnIdAsync(Parent parent)
        {
            await _context.Parents.AddAsync(parent);
            await _context.SaveChangesAsync();
            return parent.ParentID; // ✅ Return the new ParentID after insertion
        }

        // ✅ Update an existing parent
        public async Task<bool> UpdateAsync(Parent parent)
        {
            _context.Parents.Update(parent);
            var changes = await _context.SaveChangesAsync();
            return changes >= 0; // Returns true even if 0 rows were affected
        }

        // ✅ Delete a parent
        public async Task<bool> DeleteAsync(int id)
        {
            var parent = await _context.Parents.FindAsync(id);
            if (parent == null) return false;

            _context.Parents.Remove(parent);
            return await _context.SaveChangesAsync() > 0;
        }
    }


}
