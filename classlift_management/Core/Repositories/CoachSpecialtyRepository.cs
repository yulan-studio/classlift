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
    public class CoachSpecialtyRepository : ICoachSpecialtyRepository
    {
        
        private readonly AppDbContext _context;

        // Constructor to inject DbContext
        public CoachSpecialtyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Specialty>> GetSpecialtiesByCoachAsync(int coachId)
        {

            List< Specialty> specialties = await _context.CoachSpecialties
                .Where(cs => cs.CoachID == coachId)
                .Include(cs => cs.Specialty)
                    .Select(cs => cs.Specialty)
                       .ToListAsync();

            return specialties;

              
        }

        public async Task<List<Coach>> GetCoachesBySpecialtyAsync(int specialtyId)
        {
            List<Coach> coaches = await _context.CoachSpecialties
                .Where(cs => cs.SpecialtyID == specialtyId)
                .Include(cs => cs.Coach)
                    .Select(cs => cs.Coach)
                       .ToListAsync();

            return coaches;
        }
        

    }








}
