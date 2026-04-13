using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPaymentPackageRepository
    {
        Task<IEnumerable<PaymentPackage>> GetAllAsync();
        Task<PaymentPackage> GetByIdAsync(int packageId);
        Task<bool> AddAsync(PaymentPackage package);
        Task<bool> UpdateAsync(PaymentPackage package);
        Task<bool> RemoveAsync(int packageId);

    }
}
