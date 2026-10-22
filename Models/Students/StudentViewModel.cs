namespace KeelteKool2.Models.Students
{
    //ViewModel on vajalik kasutajale info kuvamiseks, siin on ainult need andmed,
    //mida Index, Details ja Delete vaated vajavad.
    public class StudentViewModel
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; } //optional sest index vaade ei vaja seda
        public string? LanguageLevel { get; set; }
    }
}
