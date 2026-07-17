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

        Task<bool> AddAsync(string title, string description, string address, int maxCapacity, DateTime scheduledAt, /*Decimal cost,*/ /*bool isActive,*/ string status, User user);

        Task<bool> RemoveAsync(int userId);

        Task<bool> UpdateAsync(int id, string title, string description, string address, int maxCapacity, DateTime scheduledAt,/* Decimal cost,*/ /*bool isActive,*/ string status, User user);

        Task<Activity> GetAsync(int userId);

        Task<IEnumerable<ActivityViewModel>> GetAllAsync();

        Task<IEnumerable<Activity>> GetAllActiveOpenAsync();

        Task UpdateActivityStatusToCompletedAsync();

        Task UpdateActivityStatusToCompletedAsync(AppDbContext dbContext,CancellationToken cancellationToken);

        //Task UpdateActivityStatusToCanceledAsync();

    }


}
