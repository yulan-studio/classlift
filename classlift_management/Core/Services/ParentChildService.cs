using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Core.Models;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Core.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;

namespace Core.Services
{
    public class ParentChildService : IParentChildService
    {
        private readonly IParentChildRepository _parentChildRepository;

        public ParentChildService(IParentChildRepository parentChildRepository)
        {
            _parentChildRepository = parentChildRepository;
        }

        public async Task<bool> AddParentToChild(int parentId, Parent parent, int childId, string relationship, int createdBy)
        {
            if (parentId <= 0 || childId <= 0)
                throw new ArgumentException("Invalid Parent or Child ID.");

            var newRelation = new ParentChild
            {
                ParentID = parentId,
                ChildID = childId,
                Relationship = relationship,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                CreatedDate = DateTimeHelper.GetTorontoTime(),
            };

            //newRelation.Parent = parent;

            return await _parentChildRepository.AddAsync(newRelation);
        }

        public async Task<IEnumerable<ParentChild>> GetParentsByChildIdAsync(int childId)
        {
            return await _parentChildRepository.GetByChildIdAsync(childId);
        }

        public async Task<ParentChild> GetParentByParentChildIdAsync(int parantChildId)
        {
            return await _parentChildRepository.GetByParentChildIdAsync(parantChildId);
        }

        public async Task<bool> RemoveParentFromChild(int parentChildId)
        {
            return await _parentChildRepository.DeleteAsync(parentChildId);
        }


        public async Task<bool> UpdateAsync(ParentChild parentChild)
        {
            var existingParent = await _parentChildRepository.GetByParentChildIdAsync(parentChild.ParentChildID);
            if (existingParent == null)
                throw new KeyNotFoundException("Parent not found.");

            existingParent.Parent = parentChild.Parent; 

            return await _parentChildRepository.UpdateAsync(existingParent);
        }
    }


}





