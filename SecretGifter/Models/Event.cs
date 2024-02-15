using System.ComponentModel.DataAnnotations;

namespace SecretGifter.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [Display(Name = "Group")]
        public int GroupId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // navigation properties
        public virtual Group? Group { get; set; }
    }
}