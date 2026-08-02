using Billing.Data;
using Microsoft.EntityFrameworkCore;

namespace Billing.Tests;

internal static class TestDb
{
    public static BillingDbContext Create()
    {
        var options = new DbContextOptionsBuilder<BillingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;
        return new BillingDbContext(options);
    }
}
