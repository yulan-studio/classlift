
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Contexts;
using Core.Models;
//using Core.mo

namespace Core.Interfaces
{
    public interface IUserRepository<T>:IRepository<T> where T: User
    {
        Task<T> GetByEmailAsync(string email);

       



       
     

    }
}
