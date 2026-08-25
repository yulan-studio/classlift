using Core.Contexts;
using Core.Interfaces;
using Core.Models;
using Core.Repositories;
using Core.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{


    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;

        public ActivityService(IActivityRepository activityRepository)
        {
            _activityRepository = activityRepository;
        }

        public async Task<bool> AddAsync(string title, string description, string address, int maxCapacity, ScheduleTiming timing, decimal cost, string status, User user)
        {
            if (cost < 0)
                throw new ArgumentOutOfRangeException(nameof(cost), "Cost cannot be negative.");


            // Create the activity
            var activity = new Activity
            {
                Title = title,
                Description = description,
                Address = address,
                MaxCapacity = maxCapacity,
                ScheduledAt = timing.ScheduledAtUtc,
                ScheduledLocalTime = timing.ScheduledLocalTime,
                ScheduledTimeZoneId = timing.TimeZoneId,
                Cost = cost,
                //IsActive = isActive,
                Status = status,
                CreatedBy = user.Id,
                CreatedDate = DateTime.UtcNow


            };

            // Save to the database
            return await _activityRepository.AddAsync(activity);


        }



        public async Task<bool> RemoveAsync(int id)
        {
            // Find the staff by ID
            var activity = await _activityRepository.GetAsync(id);
            if (activity == null)
            {
                throw new Exception("Activity not found.");
            }

            // Remove the staff
            return await _activityRepository.RemoveAsync(activity);
        }


        public async Task<bool> UpdateAsync(int id, string title, string description, string address, int maxCapacity, ScheduleTiming timing, decimal cost, string status, User user)
        //public async Task<bool> UpdateAsync(Activity activity)
        {
            //Find the staff by ID
            var activity = await _activityRepository.GetAsync(id);
            if (activity == null)
            {
                throw new Exception("Activity not found.");
            }

            if (cost < 0)
                throw new ArgumentOutOfRangeException(nameof(cost), "Cost cannot be negative.");

            if (await _activityRepository.HasRegistrationsAsync(id) && (activity.Cost ?? 0) != cost)
                throw new InvalidOperationException("Cost cannot be changed after the activity has registrations.");

            // Update fields
            activity.Title = title;
            activity.Description = description;
            activity.Address = address;
            activity.MaxCapacity = maxCapacity;
            activity.ScheduledAt = timing.ScheduledAtUtc;
            activity.ScheduledLocalTime = timing.ScheduledLocalTime;
            activity.ScheduledTimeZoneId = timing.TimeZoneId;
            activity.Cost = cost;
            //activity.IsActive = isActive;
            activity.Status = status;
            activity.UpdatedDate = DateTime.UtcNow;
            activity.UpdatedBy = user.Id;
            // Save changes
            return await _activityRepository.UpdateAsync(activity);
        }

        public async Task<Activity> GetAsync(int id)
        {
            // Retrieve the staff by ID
            var activity = await _activityRepository.GetAsync(id);
            if (activity == null)
            {
                throw new Exception("Activity not found.");
            }

            return activity;
        }

        public Task<bool> HasRegistrationsAsync(int activityId)
        {
            return _activityRepository.HasRegistrationsAsync(activityId);
        }


        public async Task<IEnumerable<ActivityViewModel>> GetAllAsync()
        {
            try
            {
                // Fetch all staff records from the repository
                var activityList = await _activityRepository.GetAllAsync();

                // You can add additional logic or transformations here if necessary
                return activityList;
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed (e.g., logging)
                throw new Exception("An error occurred while retrieving activity records.", ex);
            }
        }

        public async Task<IEnumerable<Activity>> GetAllActiveOpenAsync()
        {
            try
            {
                // Fetch all staff records from the repository
                var activityList = await _activityRepository.GetAllActiveOpenAsync();

                // You can add additional logic or transformations here if necessary
                return activityList;
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed (e.g., logging)
                throw new Exception("An error occurred while retrieving activity records.", ex);
            }
        }



        public async Task UpdateActivityStatusToCompletedAsync()
        {
            try
            {
                await _activityRepository.UpdateActivityStatusToCompletedAsync();
                
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed (e.g., logging)
                throw new Exception("An error occurred while updating activities to complete.", ex);
            }

        }


        public Task UpdateActivityStatusToCompletedAsync(AppDbContext dbContext, CancellationToken cancellationToken)
        {
            try
            {
                return _activityRepository.UpdateActivityStatusToCompletedAsync(dbContext, cancellationToken);

            }
            catch (Exception ex)
            {
                // Handle exceptions as needed (e.g., logging)
                throw new Exception("An error occurred while updating activities to complete.", ex);
            }

        }

        //public async Task UpdateActivityStatusToCanceledAsync()
        //{
        //    await _activityRepository.UpdateActivityStatusTocanceledAsync();
        //}





    }
}





