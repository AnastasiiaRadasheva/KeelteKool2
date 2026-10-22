using System.ComponentModel.DataAnnotations;

namespace KeelteKool2.Models.Lecturers
{
    //ViewModel on vajalik kasutajale info kuvamiseks (Index, Details ja Delete vaated).
    //Siin on ainult need andmed, mida kasutaja näha tohib (UserID jääb tahtlikult välja).
    public class LecturerViewModel
    {
        public Guid? Id { get; set; }

        [Display(Name = "Eesnimi")]
        public string FirstName { get; set; } = "";

        [Display(Name = "Perenimi")]
        public string LastName { get; set; } = "";

        [Display(Name = "Kvalifikatsioonid")]
        public string? Qualifications { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
