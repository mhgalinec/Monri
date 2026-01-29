using System.ComponentModel.DataAnnotations;

namespace Monri.Data.Models
{
    public abstract class BaseModel<TId>
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime Created { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? Deleted { get; set; }
        public Guid? DeletedBy { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;

    }
}
