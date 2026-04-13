using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces; // Import your Activity service
using Microsoft.Extensions.DependencyInjection;

namespace Core.BackendService
{
    public class RootCourseStatusUpdater : BackgroundService
   //This is for both Group Courses and Private Courses
    {
        private readonly IServiceProvider _serviceProvider;
        //private readonly IChildBalanceService _childBalanceService;

        public RootCourseStatusUpdater(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
           
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var courseEnrollmentService = scope.ServiceProvider.GetRequiredService<ICourseEnrollmentService>();
                    await courseEnrollmentService.UpdateCompletedCoursesAsync(); //To Private Courses, when SessionCount = Completed count, the Course is completed.
                }

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken); // Run every 10 minutes
            }
        }
    }
}
