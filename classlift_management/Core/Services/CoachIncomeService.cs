using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
//using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class CoachIncomeService: ICoachIncomeService
    {
        private readonly ICoachIncomeRepository _coachIncomeRepository;
        public CoachIncomeService(ICoachIncomeRepository coachIncomeRepository) 
        {
            _coachIncomeRepository = coachIncomeRepository;
        }
        public async Task<bool> UpdateCoachIncomeAsync(int enrollmentId, int updatedBy)
        {
            try
            {
                bool result = await _coachIncomeRepository.UpdateCoachIncomeAsync(enrollmentId, updatedBy);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public async Task<IEnumerable<CoachIncome>> GetCoachIncomeAsync(int coachId)
        {
            try
            {
                IEnumerable<CoachIncome> incomes = await _coachIncomeRepository.GetCoachIncomeAsync(coachId);
                return incomes;
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<CoachIncome>();
            }
        }


        public async Task<IEnumerable<CoachMonthlyIncome>> GetCoachMonthlyIncomeAsync(int coachId)
        {
            try
            {
                IEnumerable<CoachMonthlyIncome> incomes = await _coachIncomeRepository.GetCoachMonthlyIncomeAsync(coachId);
                return incomes;
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<CoachMonthlyIncome>();
            }
        }
    }
}
