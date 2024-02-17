using System.ComponentModel.DataAnnotations;
using SecretGifter.Data;
using SecretGifter.Models;

namespace SecretGifter.Models
{
    public class Group
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // navigation properties
        public virtual ICollection<GroupUser> GroupUsers { get; set; } = [];
        public virtual ICollection<Event> Events { get; set; } = [];

        public virtual ICollection<ApplicationUser> Members 
        { 
            get 
            {
                return GroupUsers.Select(gu => gu.User).ToList();
            } 
        }
    }
}