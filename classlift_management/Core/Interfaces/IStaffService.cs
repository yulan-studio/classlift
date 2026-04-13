using Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IStaffService
    {

               
        Task<bool> AddAsync(string name, string email, string password, string phone, string wechat, User user);

        Task<bool> RemoveAsync(int staffId);

        Task<bool> UpdateAsync(int staffId, string name, string email, /*string password, */  string phone, string wechat, User user);

        Task<Staff> GetAsync(int staffId);

        Task<IEnumerable<Staff>> GetAllAsync();


    }


}
