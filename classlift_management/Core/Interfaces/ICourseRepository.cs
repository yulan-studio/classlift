
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Contexts;
using Core.Models;
using Core.ViewModels;
using Microsoft.EntityFrameworkCore;
//using Core.mo

namespace Core.Interfaces
{
    public interface ICourseRepository
    {

       
        Task<IEnumerable<CourseViewModel>> GetAllAsync();


        // Get a course by ID
        Task<Course> GetAsync(int courseId);


        // Add a new course
        Task<bool> AddAsync(Course entity);


        // Update an existing course
        Task<bool> UpdateAsync(Course entity);
       

        // Delete a course by ID
       Task<bool> DeleteAsync(Course entity);
       

        // Get active courses
        Task<IEnumerable<Course>> GetActiveCoursesAsync();

        Task<IEnumerable<Course>> GetActiveGroupCoursesAsync();


        // Get courses by coach ID
        //Task<IEnumerable<Course>> GetCoursesByCoachIdAsync(int coachId);

        Task<Course> GetByCoachAsync(int coachId, bool isActive);

        Task<IEnumerable<Course>> GetActiveCoursesBySpecialtyAsync(int specialtyId);

        //Task<Course> GetActiveCourseByCoachAsync(int coachId);
        Task<IEnumerable<Course>> GetActiveCourseByCoachBySpecialtyAsync(int coachId, int specialId);

        Task<IEnumerable<Course>> GetCoursesByCoachAsync(int coachId);
    }
}
