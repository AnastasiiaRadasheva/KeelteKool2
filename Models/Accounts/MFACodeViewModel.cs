using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KeelteKool2.Models.Accounts
{
    public class MFACodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<SelectListItem> Providers { get; set; }
        [Required]
        [Display(Name = "Kood")]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
        [Display(Name = "Jäta brauser meelde")]
        public bool RememberBrowser { get; set; }
    }
}
