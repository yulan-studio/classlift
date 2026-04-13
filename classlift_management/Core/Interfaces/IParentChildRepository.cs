
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
    public interface IParentChildRepository
    {

       Task<bool> AddAsync(ParentChild parentChild);
       Task<IEnumerable<ParentChild>> GetByChildIdAsync(int childId);
       Task<IEnumerable<ParentChild>> GetByParentIdAsync(int parentId);

       Task<ParentChild> GetByParentChildIdAsync(int parentChildId);
       Task<bool> DeleteAsync(int parentChildId);

        Task<bool> UpdateAsync(ParentChild parentChild);





    }
}
