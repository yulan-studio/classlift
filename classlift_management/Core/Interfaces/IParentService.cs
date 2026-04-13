using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IParentService
    {

        Task<IEnumerable<Parent>> GetAllParentsAsync();


        // ✅ Get a parent by ID
        Task<Parent?> GetParentByIdAsync(int parentId);


        // ✅ Add a new parent
        Task<bool> AddAsync(Parent parent);

        Task<int> AddAndReturnIdAsync(Parent parent);


        // ✅ Update an existing parent
        Task<bool> UpdateAsync(Parent parent);


        // ✅ Delete a parent
        Task<bool> DeleteAsync(int parentId);
     


    }


}
