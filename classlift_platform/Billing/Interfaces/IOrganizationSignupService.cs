using Billing.Controllers.Public;
using Billing.Models;

namespace Billing.Interfaces
{
    public interface IOrganizationSignupService
    {
        Task<OrganizationSignupResult> CreateOrganizationAsync(
            PublicSignupRequest request);
    }
}
