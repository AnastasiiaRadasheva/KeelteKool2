using System.ComponentModel.DataAnnotations;

namespace KeelteKool2.Models.Accounts
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
