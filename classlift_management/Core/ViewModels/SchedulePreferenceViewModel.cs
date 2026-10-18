using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels;

public sealed class SchedulePreferenceViewModel
{
    public int CourseID { get; set; }

    [StringLength(2000, ErrorMessage = "Please keep your scheduling preference to 2,000 characters or fewer.")]
    public string? SchedulePreference { get; set; }
}
