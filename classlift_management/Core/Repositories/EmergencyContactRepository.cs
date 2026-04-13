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

    public class EmergencyContactRepository : IEmergencyContactRepository
    {
        


        private readonly AppDbContext _context;

        // Constructor to inject DbContext
        public EmergencyContactRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<bool> AddAsync(EmergencyContact entity)
        {
            try
            {
                await _context.EmergencyContacts.AddAsync(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public async Task<bool> RemoveAsync(EmergencyContact entity)
        {
            try
            {
                _context.EmergencyContacts.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        
      
        public async Task<EmergencyContact?> GetAsync(int id)
        {
            return await _context.EmergencyContacts              
                .FirstOrDefaultAsync(s => s.EmergencyContactID == id);
        }

        



    }








}
