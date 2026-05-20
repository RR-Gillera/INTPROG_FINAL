using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FINALPROJECT.Models
{
    public class Report
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ContentType { get; set; } = null!; // "Recipe", "Video", "User", "Review"

        [Required]
        public string ContentId { get; set; } = null!; // String to handle string IDs if necessary, or int

        [Required]
        public string ReportedById { get; set; } = null!;

        [ForeignKey("ReportedById")]
        public virtual User? ReportedBy { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "PENDING"; // "PENDING", "RESOLVED", "DISMISSED"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ResolvedAt { get; set; }
    }
}