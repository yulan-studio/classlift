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
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                return false; // Return failure in case of an exception
            }
        }

        public async Task UpdateActivityStatusToCompletedAsync()
        {

            var torontoNow = Core.DateTimeHelper.GetTorontoTime();

            var activities = await _context.Activities
                .Where(a => ((DateTime)a.ScheduledAt).AddDays(1) <= torontoNow /*&& a.IsActive == true*/)
                .ToListAsync();

            foreach (var activity in activities)
            {
                //activity.IsActive = false;
                activity.Status = "Completed";
            }

            await _context.SaveChangesAsync();
            
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
                            ScheduledAt = a.ScheduledAt,
                            //Cost = a.Cost,
                            MaxCapacity = a.MaxCapacity,
                            Status = a.Status,
                            RegisteredChildrenCount = _context.ActivityEnrollments
                                .Count(e => e.ActivityID == a.ActivityID) // Count registered children
                        })
                        .ToListAsync();
        }

        public async Task<IEnumerable<Activity>> GetAllActiveOpenAsync()
        {
            return await _context.Activities
                .Where(a => a.Status == "Open") // ✅ Only fetch active activities
                .ToListAsync();
        }









    }








}
