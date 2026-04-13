using Core.Interfaces;
using Core.Models;
using Core.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;




namespace Core.Repositories
{
    public class PaymentPackageRepository : IPaymentPackageRepository
    {
        private readonly AppDbContext _context;

        public PaymentPackageRepository(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 Get all payment packages
        public async Task<IEnumerable<PaymentPackage>> GetAllAsync()
        {
            return await _context.PaymentPackages.ToListAsync();
        }

        // 🔹 Get payment package by ID
        public async Task<PaymentPackage> GetByIdAsync(int packageId)
        {
            return await _context.PaymentPackages.FindAsync(packageId);
        }

        // 🔹 Add a new payment package
        public async Task<bool> AddAsync(PaymentPackage package)
        {
            try
            {
                await _context.PaymentPackages.AddAsync(package);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // 🔹 Update an existing payment package
        public async Task<bool> UpdateAsync(PaymentPackage package)
        {
            try
            {
                _context.PaymentPackages.Update(package);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // 🔹 Delete a payment package
        public async Task<bool> RemoveAsync(int id)
        {
            var package = await GetByIdAsync(id);
            if (package == null) return false;

            try
            {
                _context.PaymentPackages.Remove(package);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
