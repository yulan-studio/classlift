
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Contexts;
using Core.Models;
using Microsoft.EntityFrameworkCore;
//using Core.mo

namespace Core.Interfaces
{
    public interface IChildRepository
    {
        Task<IEnumerable<Child>> GetAllAsync();
        Task<Child?> GetAsync(int userId);

        Task<Child?> GetByIdAsync(int userId);

        //Task<Child?> GetChildByIdAsync(int childId);
        Task<bool> AddAsync(Child entity);
        Task<bool> UpdateAsync(Child entity);
        //Task<bool> RemoveAsync(int userId);
        Task<bool> RemoveAsync(Child entity);

        Task<bool> CheckRegisteredAsync(int childId);

        Task<bool> CheckPaidAsync(int childId);
    }
}
