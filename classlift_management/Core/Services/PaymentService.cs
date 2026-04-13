using Core.Interfaces;
using Core.Models;
using Core.Repositories;
using Core.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core.Services
{
    public class PaymentService : IPaymentService
    {

        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository<User> _userRepository;
        private readonly IChildRepository _childRepository;
        private readonly IParentRepository _parentRepository;
        private readonly IFeeRepository _feeRepository;

        public PaymentService(IPaymentRepository paymentRepository, IUserRepository<User> userRepository, IChildRepository childRepository, IParentRepository parentRepository, IFeeRepository feeRepository)
        {
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
            _childRepository = childRepository;
            _parentRepository = parentRepository;
            _feeRepository = feeRepository;
        }

        // 🔹 Get all payments
        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _paymentRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Payment>> GetByChildAsync(int childId)
        {
            return await _paymentRepository.GetByChildAsync(childId);
        }

        // 🔹 Get payment by ID
        public async Task<Payment> GetByIdAsync(int id)
        {
            return await _paymentRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Payment>> GetByPackageAsync(int packageId)
        {
            return await _paymentRepository.GetByPackageAsync(packageId);
        }


        public async Task<List<UnpaidItemViewModel>> GetUnpaidDirectEnrollmentsByChildAsync(int childId)
        {             
            return await _paymentRepository.GetUnpaidDirectEnrollmentsByChildAsync(childId);
        }

        public async Task<List<UnpaidItemViewModel>> GetUnpaidOAPEnrollmentsByChildAsync(int childId)
        {
            return await _paymentRepository.GetUnpaidOAPEnrollmentsByChildAsync(childId);
        }


        //public async Task<Child> GetChildByIdAsync(int childId)
        //{
        //    return await _paymentRepository.GetChildByIdAsync(childId);
        //}


        public async Task<IEnumerable<Parent>> GetParentsByChildAsync(int childId)
        {
            return await _paymentRepository.GetParentsByChildAsync(childId);
        }

        public async Task<IEnumerable<PaymentPackage>> GetAllActivePackagesAsync()
        {
            return await _paymentRepository.GetAllActivePackagesAsync();
        }

        // 🔹 Add a new payment
        public async Task<int> AddTokenPaymentAsync(int childId, int parentId, int? packageId, decimal amount, DateTime? paymentDate, string receiptPath, User user)
        {
            
           // var createdBy = 1;
           

            var child = await _childRepository.GetAsync(childId);
            if (child == null)
            {
                throw new Exception("Child is not found.");
            }

            var parent = await _parentRepository.GetAsync(parentId);
            if (parent == null)
            {
                throw new Exception("Parent is not found.");
            }

            //var createdByUser = await _userRepository.GetAsync(createdBy);
            //if (createdByUser == null)
            //{
            //    throw new Exception("No createdBy is added.");
            //}

            var payment = new Payment
            {
                //ChildID = childId,
                ParentID = parentId,
                CreatedBy = user.Id,
                PaymentPackageID = packageId,
                Amount = amount,
                Parent = parent,
                Child = child,
                PaymentDate = paymentDate,
                CreatedByUser = user,
                Receipt = receiptPath,
                CreatedDate = DateTimeHelper.GetTorontoTime()

            };

            // Add the course to the repository
            try
            {
                return await _paymentRepository.AddPaymentAsync(payment);
                
            }
            catch (Exception ex)
            {
                throw new Exception("No payment is added.");
            }
            
        }


        public async Task<int> AddNoneTokenPaymentAsync(int childId, int parentId, int? feeId, decimal amount, DateTime? paymentDate, string receiptPath, User user)
        {
            //return await _paymentRepository.AddDirectPaymentAsync(feeId, createdBy);

            //var child = await _childRepository.GetAsync(childId);
            //if (child == null)
            //{
            //    throw new Exception("Child is not found.");
            //}

            var child = await _childRepository.GetAsync(childId);
            if (child == null)
            {
                throw new Exception("Child is not found.");
            }

            var parent = await _parentRepository.GetAsync(parentId);
            if (parent == null)
            {
                throw new Exception("Parent is not found.");
            }

           
            var fee = await _feeRepository.GetAsync((int)feeId);
            if (fee == null)
            {
                throw new Exception("Fee is not found.");
            }

            //if (fee == null) return false;

            fee.IsPaid = true;

            // 1. Add payment record
            //var payment = new Payment
            //{
            //    FeeID = fee.FeeID,
            //    Amount = fee.TotalCost,
            //    CreatedBy = createdBy,
            //    CreatedDate = DateTime.UtcNow
            //};

            var payment = new Payment
            {
                ChildID = childId,
                ParentID = parentId,
                CreatedBy = user.Id,
                //PaymentPackageID = packageId,
                FeeID = feeId,
                Amount = amount,
                Parent = parent,
                Child = child,
                PaymentDate = paymentDate,
                CreatedByUser = user,
                Receipt = receiptPath,
                Fee = fee,
                CreatedDate = DateTimeHelper.GetTorontoTime()

            };

            try
            {
                return await _paymentRepository.AddPaymentAsync(payment);

            }
            catch (Exception ex)
            {
                throw new Exception("No payment is added.");
            }

            

        }
        // 🔹 Update an existing payment
        public async Task<bool> UpdateAsync(Payment payment)
        {
            var existingPayment = await GetByIdAsync(payment.PaymentID);
            if (existingPayment == null)
                throw new Exception("Payment not found.");

            return await _paymentRepository.UpdateAsync(payment);
        }

        // 🔹 Delete a payment
        public async Task<bool> RemoveAsync(int paymentId)
        {
            var payment = await GetByIdAsync(paymentId);
            if (payment == null)
                throw new Exception("Payment not found.");

            return await _paymentRepository.RemoveAsync(paymentId);
        }
    }
}





