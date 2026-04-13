using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Core.Interfaces
{
    public interface IAdminService
    {

        Task<bool> AddAsync(string name, string email, string password, string phone, string wechat, User user);

        Task<bool> RemoveAsync(int adminId);

        Task<bool> UpdateAsync(int adminId, string name, string email, /*string password, */  string phone, string wechat, User user);

        Task<Admin> GetAsync(int adminId);

        Task<IEnumerable<Admin>> GetAllAsync();
    }
}
