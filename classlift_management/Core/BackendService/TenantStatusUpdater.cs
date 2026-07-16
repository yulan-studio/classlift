using Core.Contexts;
using Core.Interfaces; // Import your Activity service
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.BackendService
{
    public interface ICurrentTenant
    {
        int? TenantId { get; }
        string? ConnectionString { get; }

        void SetTenant(int tenantId, string connectionString);
    }

    public sealed class CurrentTenant : ICurrentTenant
    {
        public int? TenantId { get; private set; }

        public string? ConnectionString { get; private set; }

        public void SetTenant(
            int tenantId,
            string connectionString)
        {
            TenantId = tenantId;
            ConnectionString = connectionString;
        }
    }

    public class TenantStatusUpdater : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(10);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ActivityStatusUpdater> _logger;
        private readonly ITenantConnectionStringFactory _connectionFactory;


        public TenantStatusUpdater(IServiceScopeFactory scopeFactory,
                                   ITenantConnectionStringFactory connectionFactory,
                                    ILogger<ActivityStatusUpdater> logger)
        {
            _scopeFactory = scopeFactory;
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Run immediately when the application starts.
            await RunSafelyAsync(stoppingToken);

            using var timer = new PeriodicTimer(Interval);

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await RunSafelyAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                // Normal application shutdown.
            }


            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    using (var scope = _serviceProvider.CreateScope())
            //    {
            //        var activityEnrollmentService = scope.ServiceProvider.GetRequiredService<IActivityEnrollmentService>();
            //        var enrollments = await activityEnrollmentService.UpdateActivityStatusToCompletedAsync();


                   
            //        var activityService = scope.ServiceProvider.GetRequiredService<IActivityService>();
            //        await activityService.UpdateActivityStatusToCompletedAsync();
            //    }

            //    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken); // Run every 10 minutes
            //}
        }


        private async Task RunSafelyAsync(CancellationToken cancellationToken)
        {
            try
            {
                await ProcessAllTenantsAsync(cancellationToken);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "The activity status update cycle failed.");
            }
        }



        private async Task ProcessAllTenantsAsync(CancellationToken cancellationToken)
        {
            var tenants = await LoadActiveTenantsAsync(cancellationToken);

            foreach (var tenant in tenants)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var connectionString = _connectionFactory.BuildConnectionString(tenant.DatabaseName);

                var options = new DbContextOptionsBuilder<AppDbContext>()
                                .UseMySql(
                                    connectionString,
                                    ServerVersion.AutoDetect(connectionString))
                                .Options;

                await using var dbContext = new AppDbContext(options);

                try
                {
                    await ProcessTenantAsync(dbContext, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Activity status update failed for tenant {TenantId}",
                        tenant.OrganizationId);
                }
            }
        }



        private async Task<List<TenantRegistry>> LoadActiveTenantsAsync(CancellationToken cancellationToken)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();

            var billingDbContext = scope.ServiceProvider
                .GetRequiredService<BillingDbContext>();

            return await billingDbContext.TenantRegistries
                .AsNoTracking()
                .Where(tenant => tenant.IsActive)
                .ToListAsync(cancellationToken);
        }



        private async Task ProcessTenantAsync(  AppDbContext dbContext,
                                                CancellationToken cancellationToken)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();

            var activityEnrollmentService = scope.ServiceProvider
                .GetRequiredService<IActivityEnrollmentService>();

            var activityService = scope.ServiceProvider
                .GetRequiredService<IActivityService>();

            await activityEnrollmentService
                .UpdateActivityStatusToCompletedAsync(
                    dbContext,
                    cancellationToken);

            await activityService
                .UpdateActivityStatusToCompletedAsync(
                    dbContext,
                    cancellationToken);
        }


        
    }



}

