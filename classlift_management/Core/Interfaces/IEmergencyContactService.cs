using Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IEmergencyContactService
    {



        Task<bool> AddAsync(EmergencyContact contact);





       Task<bool> DeleteAsync(int contactId);



        Task<EmergencyContact> GetAsync(int contactId);
        



    }


}
