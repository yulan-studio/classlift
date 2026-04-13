using Core.Interfaces;
using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class FeeService: IFeeService
    {
        private readonly IFeeRepository _feeRepository;

        public FeeService(IFeeRepository feeRepository)
        {
            _feeRepository = feeRepository;
        }

        // ✅ Add a City
        public async Task<bool> AddAsync(Fee fee)
        {
           
            return await _feeRepository.AddAsync(fee);
        }

        


        public async Task<bool> AddCourseFeeAsync(int enrollmentId, string paymentModel, decimal? totalCost, string description, User user)
        {
            var fee = new Fee
            {
                CourseEnrollmentID = enrollmentId,
                PaymentModel = paymentModel,
                TotalCost = totalCost,
                Description = description,
                CreatedBy = user.Id,
                CreatedAt = DateTimeHelper.GetTorontoTime(),
               //IsPaid = false
            };

            if (totalCost == null)
                fee.IsPaid = true;
            else if (totalCost == 0)
                fee.IsPaid = true;
            else
                fee.IsPaid = false;

            return await _feeRepository.AddAsync(fee);
        }


        public async Task<bool>  AddActivityFeeAsync(int enrollmentId, string paymentModel, decimal totalCost, string description, User user)
        {
            var fee = new Fee
            {
                ActivityEnrollmentID = enrollmentId,
                PaymentModel = paymentModel,
                TotalCost = totalCost,
                Description = description,
                CreatedBy = user.Id,
                CreatedAt = DateTimeHelper.GetTorontoTime(),
                //IsPaid = false
            };
            return await _feeRepository.AddAsync(fee);
        }

        public async Task<bool> DeleteCourseFeeAsync(int enrollmentId)
        {
            if (enrollmentId <= 0)
                throw new ArgumentException("Invalid enrollment ID.");

            try
            {
                return await _feeRepository.DeleteCourseFeeAsync(enrollmentId);
              
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting course fee: {ex.Message}", ex);
            }
        }

        
        public async Task<bool> DeleteActivityFeeAsync(int enrollmentId)
        {
            if (enrollmentId <= 0)
                throw new ArgumentException("Invalid enrollment ID.");

            try
            {
                return await _feeRepository.DeleteActivityFeeAsync(enrollmentId);

            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting course fee: {ex.Message}", ex);
            }
        }

        public async Task<Fee?> GetFeeForCourseEnrollmentAsync(int courseEnrollmentId)
        {
            return await _feeRepository.GetByCourseEnrollmentIdAsync(courseEnrollmentId);
        }

        public async Task<Fee?> GetByChildIdCourseIdAsync(int childId, int courseId)
        {
             return await _feeRepository.GetByChildIdCourseIdAsync(childId, courseId);
        }

        public async Task<Fee?> GetFeeForActivityEnrollmentAsync(int courseEnrollmentId)
        {
            return await _feeRepository.GetByActivityEnrollmentIdAsync(courseEnrollmentId);
        }

        public async Task<Fee?> GetByChildIdActivityIdAsync(int childId, int activityId)
        {
            return await _feeRepository.GetByChildIdActivityIdAsync(childId, activityId);
        }


        public async Task<bool> UpdateFeeAsync(Fee fee, string description, decimal? totalCost, int userId)
        {
            if (fee == null)
                return false;

            fee.Description = description;
            fee.TotalCost = totalCost;
            fee.UpdatedAt = DateTimeHelper.GetTorontoTime();
            fee.UpdatedBy = userId;

            await _feeRepository.UpdateAsync(fee);
            return true;
        }



        // ✅ Update a City
        //public async Task<bool> UpdateAsync(Fee fee)
        //{
        //    var existingFee = await _feeRepository.GetAsync(fee.FeeID);
        //    if (existingFee == null)
        //        throw new KeyNotFoundException("Fee not found.");

        //    existingFee.Description = fee.Description; // Update fields
        //    existingFee.TotalCost = fee.TotalCost;
        //    existingFee.CreatedAt = fee.CreatedAt;
        //    existingFee.IsPaid = fee.IsPaid;
        //    existingFee.PaidAt = fee.PaidAt;

        //    return await _feeRepository.UpdateAsync(fee);
        //}

        // ✅ Delete a City
        public async Task<bool> DeleteAsync(int feeId)
        {
            var fee = await _feeRepository.GetAsync(feeId);
            if (fee == null)
                throw new KeyNotFoundException("Fee not found.");
            return await _feeRepository.RemoveAsync(fee);
        }

        // ✅ Get City by ID
        public async Task<Fee> GetAsync(int feeId)
        {
            return await _feeRepository.GetAsync(feeId) ?? throw new KeyNotFoundException("Fee not found.");
        }

        // ✅ Get All Cities
        public async Task<IEnumerable<Fee>> GetAllAsync()
        {
            try
            {
                // Fetch all coach records from the repository
                var feeList = await _feeRepository.GetAllAsync();

                // You can add additional logic or transformations here if necessary
                return feeList;
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed (e.g., logging)
                throw new Exception("An error occurred while retrieving fee records.", ex);
            }
        }




        public async Task<bool> UpdateActivityIsPaidAsync(int activityEnrollmentID, int userId)
        {
            return await _feeRepository.UpdateActivityIsPaidAsync(activityEnrollmentID, userId);
        }


        public async Task<bool> UpdateCourseIsPaidAsync(int courseEnrollmentID, int userId)
        {
            return await _feeRepository.UpdateCourseIsPaidAsync(courseEnrollmentID, userId);
        }

        public async Task<bool> MarkFeeAsUnpaidAsync(int feeId)
        {
            try
            {
                var fee = await _feeRepository.GetAsync(feeId);
                
                if (fee == null)
                    return false;


                fee.IsPaid = false;
                return await _feeRepository.UpdateAsync(fee);

            }
            catch (Exception ex)
            {
                // Optionally log the exception
                //Console.WriteLine($"Error marking fee as unpaid: {ex.Message}");
                return false;
            }
        }





    }
}
