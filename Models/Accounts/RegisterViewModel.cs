using System.ComponentModel.DataAnnotations;

namespace KeelteKool2.Models.Accounts
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Kirjuta Parool Uuesti")]
        [Compare("Password", ErrorMessage = "Paroolid ei ühti.")]
        public string ConfirmPassword { get; set; }
        public string PlaceHolder { get; set; }

        public RegisterStatus AccountStatus { get; set; } = RegisterStatus.Pending;
    }
}
