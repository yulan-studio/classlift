using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models;

/// <summary>A single independently scheduled occurrence in an Activity series.</summary>
public class ActivitySession
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ActivitySessionID { get; set; }
    public int ActivityID { get; set; }
    [ForeignKey(nameof(ActivityID))]
    public virtual Activity Activity { get; set; } = null!;
    // Optional per-session contact; when empty, the series contact may be used as the default.
    public int? ContactID { get; set; }
    [ForeignKey(nameof(ContactID))]
    public virtual User? Contact { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? ScheduledLocalTime { get; set; }
    [MaxLength(100)] public string? ScheduledTimeZoneId { get; set; }
    public decimal ScheduledHours { get; set; }
    public int MaxCapacity { get; set; }
    [Column(TypeName = "decimal(10, 2)")] public decimal Cost { get; set; }
    [Required, MaxLength(40)] public string Status { get; set; } = "Open";
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public virtual ICollection<ActivityEnrollment> Enrollments { get; set; } = new List<ActivityEnrollment>();
}
