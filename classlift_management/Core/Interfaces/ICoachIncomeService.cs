using Core.DTOs;
using Core.Models;
using Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICoachIncomeService
    {

        Task<bool> UpdateCoachIncomeAsync(int enrollmentId, int updatedBy);

        Task<IEnumerable<CoachIncome>> GetCoachIncomeAsync(int coachId);

        Task<IEnumerable<CoachMonthlyIncome>> GetCoachMonthlyIncomeAsync(int coachId);

    }


}
