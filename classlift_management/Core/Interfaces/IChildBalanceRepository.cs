using System;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Core.ViewModels;

namespace Core.Interfaces
{
    public interface IChildBalanceRepository 
    {
        Task<bool> AddBalanceAsync(Core.Models.ChildBalance balance);
        Task<bool> AddPaymentToBalanceAsync(int childId, int paymentId, decimal amount, int createdBy);

        Task<bool> RemovePaymentToBalanceAsync(int childId, int paymentId, int createdBy);

        Task<bool> DeductCourseSessionCostAsync(int enrollmentId, int createdBy);

        Task<bool> DeductCourseCostAsync(int childId, int courseId, decimal cost, int createdBy);

        Task<bool> DeductActivityCostAsync(int childId, int activityId, decimal cost, int createdBy);

        Task<bool> DeductGroupCourseCostAsync(int childId, int courseId, decimal cost, int createdBy);

        Task<List<Core.ViewModels.ChildBalance>> GetBalanceHistoryAsync(int childId);

        Task<decimal> GetFinalBalanceAsync(int childId);
    }
}
