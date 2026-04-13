using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFeeService
    {
        Task<bool> AddAsync(Fee fee);

        Task<bool> AddCourseFeeAsync(int enrollmentId, string paymentModel, decimal? totalCost, string description, User user);

        Task<bool> DeleteCourseFeeAsync(int enrollmentId);

        Task<bool> DeleteActivityFeeAsync(int enrollmentId);

        Task<bool> AddActivityFeeAsync(int enrollmentId, string paymentModel, decimal totalCost, string description, User user);

        //Task<bool> UpdateAsync(Fee fee);

        Task<bool> UpdateFeeAsync(Fee fee, string description, decimal? totalCost, int userId);

        Task<bool> DeleteAsync(int feeId);
       
        Task<Fee> GetAsync(int feeId);

        Task<Fee?> GetFeeForCourseEnrollmentAsync(int courseEnrollmentId);

        Task<Fee?> GetByChildIdCourseIdAsync(int childId, int courseId);

        Task<Fee?> GetFeeForActivityEnrollmentAsync(int courseEnrollmentId);

        Task<Fee?> GetByChildIdActivityIdAsync(int childId, int activityId);

        Task<IEnumerable<Fee>> GetAllAsync();


        Task<bool> UpdateActivityIsPaidAsync(int activityEnrollmentID, int userId);

        Task<bool> UpdateCourseIsPaidAsync(int courseEnrollmentID, int userId);

        Task<bool> MarkFeeAsUnpaidAsync(int feeId);


    }


}
