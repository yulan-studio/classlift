using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Core.Models;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Core.Repositories;
using System.Numerics;
using System.Xml.Linq;

namespace Core.Services
{
    public class PaymentPackageService : IPaymentPackageService
    {

        private readonly IPaymentPackageRepository _paymentPackageRepository;

        public PaymentPackageService(IPaymentPackageRepository paymentPackageRepository)
        {
            _paymentPackageRepository = paymentPackageRepository;
        }

        // 🔹 Get all payment packages
        public async Task<IEnumerable<PaymentPackage>> GetAllAsync()
        {
            return await _paymentPackageRepository.GetAllAsync();
        }

        // 🔹 Get payment package by ID
        public async Task<PaymentPackage> GetByIdAsync(int packageId)
        {
            return await _paymentPackageRepository.GetByIdAsync(packageId);
        }

        // 🔹 Add a new payment package
        public async Task<bool> AddAsync(PaymentPackage package)
        {
            if (string.IsNullOrWhiteSpace(package.Title))
                throw new ArgumentException("Package title cannot be empty.");

            if (package.Amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            return await _paymentPackageRepository.AddAsync(package);
        }

        // 🔹 Update an existing payment package
        public async Task<bool> UpdateAsync(PaymentPackage package)
        {
            //var existingPackage = await GetByIdAsync(package.PackageID);
            //if (existingPackage == null)
            //    throw new Exception("Payment package not found.");

            return await _paymentPackageRepository.UpdateAsync(package);
        }

        // 🔹 Delete a payment package
        public async Task<bool> RemoveAsync(int packageId)
        {
            var package = await GetByIdAsync(packageId);
            if (package == null)
                throw new Exception("Payment package not found.");

            return await _paymentPackageRepository.RemoveAsync(packageId);
        }
    }
}





