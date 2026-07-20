using Core.Contexts;
using Core.Models;
using Core.Repositories;
using Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICourseService
    {


        Task<IEnumerable<CourseViewModel>> GetAllAsync();


        // Get a course by ID
        Task<Course> GetAsync(int courseId);


        // Add a new course

        Task<bool> AddAsync(string title, string? description, string courseType, int? maxCapatcity, int? sessionCount, decimal? hourlyCost, decimal? hourlyCost2, bool isActive, int coachId, int specialtyId,  User user);




        // Update an existing course
        Task<bool> UpdateAsync(int courseId, string title, string? description, string courseType, int? maxCapatcity, int? sessionCount, decimal hourlyCost, decimal? hourlyCost2, bool isActive, User user/*, int userId, int updatedBy*/);


        // Delete a course by ID
        Task<bool> RemoveAsync(int courseId);


        // Get active courses
        Task<IEnumerable<Course>> GetActiveCoursesAsync();

        Task<IEnumerable<Course>> GetActiveGroupCoursesAsync();

        Task<IEnumerable<Course>> GetActiveGroupCoursesAsync(AppDbContext dbContext, CancellationToken cancellationToken);


        // Get courses by coach ID
        //Task<IEnumerable<Course>> GetCoursesByCoachIdAsync(int coachId);

        Task<IEnumerable<Course>> GetActiveCoursesBySpecialtyAsync(int specialtyId);

        Task<IEnumerable<Course>> GetActiveCourseByCoachBySpecialtyAsync(int coachId, int specialtyId);

        Task<IEnumerable<Course>> GetCoursesByCoachAsync(int coachId);

    }


}
