using System.ComponentModel.DataAnnotations;

namespace KeelteKool2.Models.Accounts
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Emailiaadress")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Parool")]
        public string Password { get; set; }

        [Display(Name = "Mäleta sisselogitust")]
        public bool RememberMe { get; set; }
    }
}
