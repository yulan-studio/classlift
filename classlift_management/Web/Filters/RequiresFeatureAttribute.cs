using Microsoft.AspNetCore.Mvc;

namespace Web.Filters
{
    /// <summary>
    /// Requires the current tenant's plan to contain the specified feature.
    /// This attribute complements user/role authorization; it does not replace it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class RequiresFeatureAttribute : TypeFilterAttribute
    {
        public RequiresFeatureAttribute(string featureKey)
            : base(typeof(RequiresFeatureFilter))
        {
            Arguments = new object[] { featureKey };
        }
    }
}
