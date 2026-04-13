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
    public interface IChildBalanceService
    {
        Task<bool> AddBalanceFixAsync(int childId, string actionType, decimal amount, string remarks, string? calculationPath, int createdBy);
        Task<bool> AddPaymentToBalanceAsync(int childId, int paymentId, decimal amount, int createdBy);
        Task<bool> RemovePaymentToBalanceAsync(int childId, int paymentId, int createdBy);

        //This is for private courses where cost is calculated per session

        Task<bool> DeductCourseCostAsync(int childId, int courseId, decimal cost, int createdBy);
        Task<bool> DeductCourseSessionCostAsync(int enrollmentId, int createdBy);
        Task<bool> DeductActivityCostAsync(int childId, int activityId, decimal cost, int createdBy);

        Task<bool> DeductGroupCourseCostAsync(int childId, int courseId, decimal cost, int createdBy);
        //Task<IEnumerable<ChildBalanceViewModel>> GetBalanceHistoryAsync(int childId);
        Task<IEnumerable<Core.ViewModels.ChildBalance>> GetBalanceHistoryAsync(int childId);
        Task<decimal> GetFinalBalanceAsync(int childId);
    }


}
