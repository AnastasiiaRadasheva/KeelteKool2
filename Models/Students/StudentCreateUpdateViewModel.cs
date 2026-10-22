namespace KeelteKool2.Models.Students
{
    public class StudentCreateUpdateViewModel
    {
        public Guid? Id { get; set; } //optional sest uuel õpilasel id-d veel ei ole
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? LanguageLevel { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
