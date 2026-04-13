using Core.Interfaces;
using Core.Models;
using Core.Repositories;
using Core.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{


    public class ChildBalanceService: IChildBalanceService
    {

        private readonly IChildBalanceRepository _balanceRepository;

        public ChildBalanceService(IChildBalanceRepository balanceRepository)
        {
            _balanceRepository = balanceRepository;
        }


        public async Task<bool> AddBalanceFixAsync(int childId, string actionType, decimal amount, string remarks, string? calculationPath, int createdBy)
        {
            // Get the current balance
            var currentBalance = await _balanceRepository.GetFinalBalanceAsync(childId);

            //decimal balanceChange = actionType == "Refund" ? -amount : amount;
            decimal balanceChange = amount;
            decimal newBalance = currentBalance + balanceChange;

            var newTransaction = new Core.Models.ChildBalance
            {
                ChildID = childId,
                TransactionType = actionType,
                Remarks = remarks,
                BalanceChange = balanceChange,
                Balance = newBalance,
                Calculation = calculationPath,
                CreatedDate = DateTime.Now,
                CreatedBy = createdBy
            };

            return await _balanceRepository.AddBalanceAsync(newTransaction);
        }

        //When a parent buys a payment package
        public async Task<bool> AddPaymentToBalanceAsync(int childId, int paymentId, decimal amount, int createdBy)
        {
            try
            {
                bool result = await _balanceRepository.AddPaymentToBalanceAsync(childId, paymentId, amount, createdBy);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

       

        public async Task<bool> RemovePaymentToBalanceAsync(int childId, int paymentId, int createdBy)
        {
            try
            {
                bool result = await _balanceRepository.RemovePaymentToBalanceAsync(childId, paymentId, createdBy);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        //After a child finishes a course session (Token Payment)
        public async Task<bool> DeductCourseSessionCostAsync(int enrollmentId,int createdBy)
        {
            try
            {
                bool result = await _balanceRepository.DeductCourseSessionCostAsync(enrollmentId, createdBy);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //After a child confirms participation in private course (Token Payment)
        public async Task<bool> DeductCourseCostAsync(int childId, int courseId, decimal cost, int createdBy)
        {
            try
            {
                bool result = await _balanceRepository.DeductCourseCostAsync(childId, courseId, cost, createdBy);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //After a child confirms participation in an activity (Token Payment)
        public async Task<bool> DeductActivityCostAsync(int childId, int activityId, decimal cost, int createdBy)
        {
            try
            {
                bool result = await _balanceRepository.DeductActivityCostAsync(childId, activityId, cost, createdBy);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeductGroupCourseCostAsync(int childId, int courseId, decimal cost, int createdBy)
        {
            try
            {
                bool result = await _balanceRepository.DeductGroupCourseCostAsync(childId, courseId, cost, createdBy);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Core.ViewModels.ChildBalance>> GetBalanceHistoryAsync(int childId)
        {
            try
            {
                var balances = await _balanceRepository.GetBalanceHistoryAsync(childId);
                return balances;
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<Core.ViewModels.ChildBalance>();
            }
            
        }




        public async Task<decimal> GetFinalBalanceAsync(int childId)
        {
            try
            {
                var balance = await _balanceRepository.GetFinalBalanceAsync(childId);
                return balance;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }


}