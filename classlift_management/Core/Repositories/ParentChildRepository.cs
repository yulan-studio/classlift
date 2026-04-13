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

    public class ParentChildRepository : IParentChildRepository
    {
        private readonly AppDbContext _context;

        public ParentChildRepository(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Add a parent-child relationship
        public async Task<bool> AddAsync(ParentChild parentChild)
        {
            await _context.ParentChild.AddAsync(parentChild);
            return await _context.SaveChangesAsync() > 0;
        }

        // ✅ Get all parents for a specific child
        public async Task<IEnumerable<ParentChild>> GetByChildIdAsync(int childId)
        {
           
                return await _context.ParentChild
               .Include(pc => pc.Parent)
               .Where(pc => pc.ChildID == childId)
               .ToListAsync();
           
           
        }

        // ✅ Get all children for a specific parent
        public async Task<IEnumerable<ParentChild>> GetByParentIdAsync(int parentId)
        {
            return await _context.ParentChild
                .Include(pc => pc.Child)
                .Where(pc => pc.ParentID == parentId)
                .ToListAsync();
        }


        public async Task<ParentChild> GetByParentChildIdAsync(int parentChildId)
        {

            return await _context.ParentChild.Include(pc => pc.Parent).Where(pc => pc.ParentChildID == parentChildId).FirstOrDefaultAsync();

           


        }

        // ✅ Delete a parent-child relationship
        public async Task<bool> DeleteAsync(int parentChildId)
        {
            var parentChild = await _context.ParentChild.FindAsync(parentChildId);
            if (parentChild == null) return false;

            _context.ParentChild.Remove(parentChild);
            return await _context.SaveChangesAsync() > 0;
        }



        public async Task<bool> UpdateAsync(ParentChild entity)
        {
            try
            {
                _context.ParentChild.Update(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }


}
