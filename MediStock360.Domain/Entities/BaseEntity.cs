using System.ComponentModel.DataAnnotations;

namespace MediStock360.Domain.Entities
{
    public abstract class BaseEntity
    {
        public bool? IsActive { get; set; } = true;
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public long? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public long? UpdatedBy { get; set; }

        public bool? IsSynced { get; set; } = false;
    }
}
