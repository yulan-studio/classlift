
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
    public interface IParentRepository
    {

        Task<IEnumerable<Parent>> GetAllAsync();
        Task<Parent?> GetAsync(int parentId);
        Task<bool> AddAsync(Parent parent);

        Task<int> AddAndReturnIdAsync(Parent parent);
        
        Task<bool> UpdateAsync(Parent parent);
        Task<bool> DeleteAsync(int parentId);


    }
}
