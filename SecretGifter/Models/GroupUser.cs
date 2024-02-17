using SecretGifter.Data;

namespace SecretGifter.Models
{
    public class GroupUser
    {
        public string UserId { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public bool IsAdmin { get; set; } = false;

        // navigation properties
        public virtual ApplicationUser User { get; set; } = new ApplicationUser();
        public virtual Group Group { get; set; } = new Group();
    }
}