using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces; // Import your Activity service
using Microsoft.Extensions.DependencyInjection;

namespace Core.BackendService
{
    public class GroupCourseStatusUpdater : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IChildBalanceService _childBalanceService;

        public GroupCourseStatusUpdater(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
           
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var courseService = scope.ServiceProvider.GetRequiredService<ICourseService>();
                    var courses = await courseService.GetActiveGroupCoursesAsync();


                    var courseEnrollmentService = scope.ServiceProvider.GetRequiredService<ICourseEnrollmentService>();
                    foreach (var course in courses)
                    {
                        //Automatically update children's group course sessions to "Completed" after their scheduled time.
                        await courseEnrollmentService.UpdateChildCompletedSessionsAsync(course.CourseID);
                        //Automatically update group course sessions to "Completed" after their scheduled time.
                        await courseEnrollmentService.UpdateCompletedSessionsAsync(course.CourseID);
                    }
                    
                   
                }

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken); // Run every 10 minutes
            }
        }
    }
}
