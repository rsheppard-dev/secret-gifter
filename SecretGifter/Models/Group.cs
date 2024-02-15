using System.ComponentModel.DataAnnotations;
using SecretGifter.Data;
using SecretGifter.Models;

namespace SecretGifter.Models
{
    public class Group
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // navigation properties
        public virtual ICollection<ApplicationUser> Members { get; set; } = [];
        public virtual ICollection<Event> Events { get; set; } = [];
    }
}