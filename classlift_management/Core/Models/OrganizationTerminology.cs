namespace Core.Models
{
    public sealed class OrganizationTerminology
    {
        public string OrganizationType { get; set; } = "Coaching";

        public string ProviderSingular { get; set; } = "Coach";

        public string ProviderPlural { get; set; } = "Coaches";

        public string ParticipantSingular { get; set; } = "Child";

        public string ParticipantPlural { get; set; } = "Children";
    }
}
