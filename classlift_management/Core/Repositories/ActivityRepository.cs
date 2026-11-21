using Core.Interfaces;
using Core.Models;
using Core.ViewModels;
using Core.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;




namespace Core.Repositories
{

    public class ActivityRepository : IActivityRepository
    {
        private readonly AppDbContext _context;

        // Constructor to inject DbContext
        public ActivityRepository(AppDbContext context)
        {
            _context = context;
        }


        // Add a new activity to the database asynchronously
        public async Task<bool> AddAsync(Activity entity)
        {
            try
            {
                await _context.Activities.AddAsync(entity);
                await _context.SaveChangesAsync();  // Commit the changes asynchronously
                return true;
            }
            catch (Exception)
            {
                return false; // Return failure in case of an exception
            }
        }

        // Remove a activity from the database asynchronously
        public async Task<bool> RemoveAsync(Activity entity)
        {
            try
            {
                _context.Activities.Remove(entity);
                await _context.SaveChangesAsync();  // Commit the changes asynchronously
                return true;
            }
            catch (Exception)
            {
                return false; // Return failure in case of an exception
            }
           
        }

        // Update an existing activity in the database asynchronously
        public async Task<bool> UpdateAsync(Activity entity)
        {
            try
            {
                _context.Activities.Update(entity);
                await _context.SaveChangesAsync();  // Commit the changes asynchronously
                return true;
            }
            catch (Exception)
            {
                return false; // Return failure in case of an exception
            }
        }

        public async Task UpdateActivityStatusToCompletedAsync()
        {

            var torontoNow = DateTime.UtcNow;
            var activities = await _context.ActivitySessions
                .Where(s => s.ScheduledHours > 0 && s.ScheduledAt.AddHours((double)s.ScheduledHours) <= torontoNow && s.Status == "Open")
                .ToListAsync();
            foreach (var session in activities)
            {
                session.Status = "Completed";
                var series = await _context.Activities.FindAsync(session.ActivityID);
                if (series != null) series.IsActive = true;
            }

            await _context.SaveChangesAsync();
            
        }



        public async Task UpdateActivityStatusToCompletedAsync(AppDbContext dbContext,CancellationToken cancellationToken)
        {

            var torontoNow = DateTime.UtcNow;

            var activities = await dbContext.ActivitySessions
                .Where(s => s.ScheduledHours > 0 && s.ScheduledAt.AddHours((double)s.ScheduledHours) <= torontoNow && s.Status == "Open")
                .ToListAsync(cancellationToken);
            foreach (var session in activities)
            {
                session.Status = "Completed";
            }

            await dbContext.SaveChangesAsync(cancellationToken);
           

        }


        // Find a activity by its email asynchronously
        //public async Task<ActivityViewModel> Get2Async(int activityId)
        //{
        //    return await _context.Activities//.FindAsync(activityId);  // Finds by ID asynchronously
        //                 .Where(a => a.ActivityID == activityId)
        //                .Select(a => new ActivityViewModel
        //                {
        //                    Title = a.Title,
        //                    Description = a.Description,
        //                    Address = a.Address,
        //                    ScheduledAt = a.ScheduledAt,
        //                    Cost = a.Cost,
        //                    Status = a.Status,
        //                    RegisteredChildrenCount = _context.ActivityEnrollments
        //                                .Count(e => e.ActivityID == a.ActivityID) // Count registered children
        //                }).FirstOrDefaultAsync();
        //}

        public async Task<Activity> GetAsync(int activityId)
        {
            return await _context.Activities.FindAsync(activityId);  // Finds by ID asynchronously
                        
        }

        public Task<bool> HasRegistrationsAsync(int activityId)
        {
            return _context.ActivityEnrollments.AnyAsync(e => e.ActivityID == activityId);
        }

        public async Task<bool> AddSessionAsync(ActivitySession session)
        {
            await _context.ActivitySessions.AddAsync(session);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all activities from the database asynchronously
        public async Task<IEnumerable<ActivityViewModel>> GetAllAsync()
        {
            //return await _context.Activities.ToListAsync();  // Retrieves all activities asynchronously

            return await _context.Activities
                        .Select(a => new ActivityViewModel
                        {
                            ActivityID = a.ActivityID,
                            Title = a.Title,
                            Description = a.Description,
                            Address = a.Address,
                            IsActive = a.IsActive,
                            ScheduleType = a.ScheduleType,
                            NextSessionAt = a.Sessions.Where(s => s.Status != "Canceled" && s.Status != "Completed").OrderBy(s => s.ScheduledAt).Select(s => (DateTime?)s.ScheduledAt).FirstOrDefault(),
                            SessionCount = a.Sessions.Count(),
                            RegisteredChildrenCount = _context.ActivityEnrollments
                                .Count(e => e.ActivityID == a.ActivityID) // Count registered children
                        })
                        .ToListAsync();
        }

        public async Task<IEnumerable<Activity>> GetAllActiveOpenAsync()
        {
            return await _context.Activities
                .Include(a => a.Sessions)
                .Where(a => a.IsActive && a.Sessions.Any(s => s.Status == "Open"))
                .Select(a => a)
                .ToListAsync();
        }









    }








}
