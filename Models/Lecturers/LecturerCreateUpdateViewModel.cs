using System.ComponentModel.DataAnnotations;

namespace KeelteKool2.Models.Lecturers
{
    //Create/Update vormi ViewModel. Id on valikuline: uue õppejõu lisamisel Id-d veel ei ole.
    //Nimed on kohustuslikud, kvalifikatsioonid võivad olla tühjad.
    public class LecturerCreateUpdateViewModel
    {
        public Guid? Id { get; set; }

        [Display(Name = "Eesnimi")]
        [Required(ErrorMessage = "Eesnimi on kohustuslik")]
        public string FirstName { get; set; } = "";

        [Display(Name = "Perenimi")]
        [Required(ErrorMessage = "Perenimi on kohustuslik")]
        public string LastName { get; set; } = "";

        [Display(Name = "Kvalifikatsioonid")]
        public string? Qualifications { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
