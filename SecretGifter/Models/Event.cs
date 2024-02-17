using System.ComponentModel.DataAnnotations;

namespace SecretGifter.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Display(Name = "Group")]
        public int GroupId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // navigation properties
        public virtual Group? Group { get; set; }
    }
}