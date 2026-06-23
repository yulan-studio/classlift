using Microsoft.AspNetCore.Mvc;

namespace Billing.Filters
{
    public class RequireFeatureAttribute : TypeFilterAttribute
    {
        public RequireFeatureAttribute(string featureKey)
            : base(typeof(RequireFeatureFilter))
        {
            Arguments = new object[] { featureKey };
        }
    }
}