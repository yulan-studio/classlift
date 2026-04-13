
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IEmergencyContactRepository
    {
       Task<bool> AddAsync(EmergencyContact entity);



        Task<bool> RemoveAsync(EmergencyContact entity);



        Task<EmergencyContact?> GetAsync(int id);
       


    }



}
