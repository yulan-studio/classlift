using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPaymentPackageService
    {
        Task<IEnumerable<PaymentPackage>> GetAllAsync();
        Task<PaymentPackage> GetByIdAsync(int id);
        Task<bool> AddAsync(PaymentPackage package);
        Task<bool> UpdateAsync(PaymentPackage package);
        Task<bool> RemoveAsync(int id);
    }
}
