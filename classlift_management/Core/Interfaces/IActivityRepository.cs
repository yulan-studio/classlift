
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Contexts;
using Core.Models;
using Core.ViewModels;
//using Core.mo

namespace Core.Interfaces
{
    public interface IActivityRepository
    {

        // Add a new Activity to the database asynchronously
        Task<bool> AddAsync(Activity entity);


        // Remove a Activity from the database asynchronously
        Task<bool> RemoveAsync(Activity entity);


        // Update an existing Activity in the database asynchronously
        Task<bool> UpdateAsync(Activity entity);


        // Find a Activity by its id asynchronously
        //Task<ActivityViewModel> GetAsync(int activityId);

        Task<Activity> GetAsync(int activityId);


        // Get all Activity from the database asynchronously
        Task<IEnumerable<ActivityViewModel>> GetAllAsync();

        Task<IEnumerable<Activity>> GetAllActiveOpenAsync();

        Task UpdateActivityStatusToCompletedAsync();

        //Task UpdateActivityStatusToCanceledAsync();

    }
}
