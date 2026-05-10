using System.ComponentModel.DataAnnotations;

namespace UserManagementApp.Models.ViewModels
{
    public class EditProfileViewModel
    {
        [Required]
        public string? Username { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }
    }
}
