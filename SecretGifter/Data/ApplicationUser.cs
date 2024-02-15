using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using SecretGifter.Models;

namespace SecretGifter.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    [NotMapped]
    public IFormFile? AvatarFormFile { get; }

    public string? AvatarFileName { get; set; }

    public string? AvatarContentType { get; set; }

    public byte[]? AvatarFileData { get; set; }


    // navigation properties
    public virtual ICollection<Group> Groups { get; set; } = [];
    public virtual ICollection<ApplicationUser> Friends { get; set; } = [];
}

