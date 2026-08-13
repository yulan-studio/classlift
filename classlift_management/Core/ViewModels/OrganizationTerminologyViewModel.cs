using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels
{
    public sealed class OrganizationTerminologyViewModel
    {
        [Required]
        [Display(Name = "Organization type")]
        public string OrganizationType { get; set; } = "Coaching";

        [Required, StringLength(40)]
        [Display(Name = "Provider role (singular)")]
        public string ProviderSingular { get; set; } = "Coach";

        [Required, StringLength(40)]
        [Display(Name = "Provider role (plural)")]
        public string ProviderPlural { get; set; } = "Coaches";

        [Required, StringLength(40)]
        [Display(Name = "Participant role (singular)")]
        public string ParticipantSingular { get; set; } = "Child";

        [Required, StringLength(40)]
        [Display(Name = "Participant role (plural)")]
        public string ParticipantPlural { get; set; } = "Children";

        [Display(Name = "Participants are under 18 or require parent/guardian support")]
        public bool ParticipantsRequireParentSupport { get; set; } = true;
    }
}
