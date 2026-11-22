using Core.Contexts;
using Core.Models;
using Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IActivityService
    {

        Task<bool> AddAsync(string title, string description, string address, string scheduleType, bool isActive, ScheduleTiming? timing, decimal? scheduledHours, int? maxCapacity, decimal? cost, string? sessionStatus, string? recurrenceFrequency, int? recurrenceCount, User user);

        Task<bool> RemoveAsync(int userId);

        Task<bool> UpdateAsync(int id, string title, string description, string address, string scheduleType, bool isActive, User user);

        Task<Activity> GetAsync(int userId);

        Task<bool> HasRegistrationsAsync(int activityId);
        Task<ActivitySession> AddSessionAsync(int activityId, ScheduleTiming timing, decimal hours, int capacity, decimal cost, string status, int userId);

        Task<IEnumerable<ActivityViewModel>> GetAllAsync();

        Task<IEnumerable<Activity>> GetAllActiveOpenAsync();

        Task UpdateActivityStatusToCompletedAsync();

        Task UpdateActivityStatusToCompletedAsync(AppDbContext dbContext,CancellationToken cancellationToken);

        //Task UpdateActivityStatusToCanceledAsync();

    }


}
