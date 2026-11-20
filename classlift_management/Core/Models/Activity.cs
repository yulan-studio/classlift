using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Activity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActivityID { get; set; }

        // The Activity row is the series. Sessions are stored separately so that
        // each occurrence can have its own roster, capacity, price and payments.
        [Required]
        [MaxLength(40)]
        public string ScheduleType { get; set; } = ActivityScheduleTypes.OneTime;

        public virtual ICollection<ActivitySession> Sessions { get; set; } = new List<ActivitySession>();

        // Read-only compatibility projections for legacy screens; these are not database columns.
        [NotMapped] public DateTime ScheduledAt => Sessions.OrderBy(s => s.ScheduledAt).Select(s => s.ScheduledAt).FirstOrDefault();
        [NotMapped] public DateTime? ScheduledLocalTime => Sessions.OrderBy(s => s.ScheduledAt).Select(s => s.ScheduledLocalTime).FirstOrDefault();
        [NotMapped] public string? ScheduledTimeZoneId => Sessions.OrderBy(s => s.ScheduledAt).Select(s => s.ScheduledTimeZoneId).FirstOrDefault();
        [NotMapped] public decimal? ScheduledHours => Sessions.OrderBy(s => s.ScheduledAt).Select(s => (decimal?)s.ScheduledHours).FirstOrDefault();
        [NotMapped] public decimal? Cost => Sessions.OrderBy(s => s.ScheduledAt).Select(s => (decimal?)s.Cost).FirstOrDefault();
        [NotMapped] public int MaxCapacity => Sessions.OrderBy(s => s.ScheduledAt).Select(s => s.MaxCapacity).FirstOrDefault();
        [NotMapped] public string Status => Sessions.OrderBy(s => s.ScheduledAt).Select(s => s.Status).FirstOrDefault() ?? "Closed";

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [MaxLength(255)]
        public string Address { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public int? ContactID { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } 

        public DateTime UpdatedDate { get; set; } 

        // Navigation properties for foreign key relationships
        [ForeignKey(nameof(ContactID))]
        public virtual User? Contact { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual User CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual User? UpdatedByUser { get; set; }
    }
}
