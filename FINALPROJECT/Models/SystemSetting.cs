using System.ComponentModel.DataAnnotations;

namespace FINALPROJECT.Models
{
    public class SystemSetting
    {
        [Key]
        public int Id { get; set; }

        // General Configuration
        [StringLength(100)]
        public string SiteName { get; set; } = "Flavorly";

        [StringLength(200)]
        public string Tagline { get; set; } = "The Heart of Home Cooking";

        public bool MaintenanceMode { get; set; } = false;

        // Content Moderation
        public bool AutoFlaggingKeywords { get; set; } = true;

        public int AutoHideReportsThreshold { get; set; } = 5;

        public bool RequireManualApproval { get; set; } = false;

        // Email (SMTP) Settings
        [StringLength(200)]
        public string? SmtpHost { get; set; }

        public int SmtpPort { get; set; } = 587;

        [StringLength(100)]
        public string? SmtpUsername { get; set; }

        [StringLength(100)]
        public string? SmtpPassword { get; set; }
    }
}
