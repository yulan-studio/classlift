using Core.Models;
using Core.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllAsync();

        Task<IEnumerable<Payment>> GetByChildAsync(int childId);
        Task<Payment> GetByIdAsync(int id);
        Task<IEnumerable<Payment>> GetByPackageAsync(int packageId);
        Task<int> AddPaymentAsync(Payment payment);

        //Task<bool> AddDirectPaymentAsync(int feeId, int createdBy);

        Task<Child> GetChildByIdAsync(int childId);

        Task<IEnumerable<Parent>> GetParentsByChildAsync(int childId);
        Task<IEnumerable<PaymentPackage>> GetAllActivePackagesAsync();
        Task<bool> UpdateAsync(Payment payment);
        Task<bool> RemoveAsync(int id);
        Task<List<UnpaidItemViewModel>> GetUnpaidDirectEnrollmentsByChildAsync(int childId);

        Task<List<UnpaidItemViewModel>> GetUnpaidOAPEnrollmentsByChildAsync(int childId);


    }
}
