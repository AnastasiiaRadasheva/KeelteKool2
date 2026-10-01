using System.ComponentModel.DataAnnotations;

namespace KeelteKool2.Models.Accounts
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Vana parool")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Uus parool")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Uus parool ja selle teine kirje ei kattu")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
