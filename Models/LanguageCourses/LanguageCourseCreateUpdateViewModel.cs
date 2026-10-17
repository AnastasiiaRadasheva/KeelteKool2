namespace KeelteKool2.Models.LanguageCourses
{
    public class LanguageCourseCreateUpdateViewModel
    {
        public Guid? Id { get; set; } //optional sest index vaade ei vaja seda
        public string Nimetus { get; set; }
        public string Keel { get; set; }
        public string? Tase { get; set; } //optional sest index vaade ei vaja seda
        public string? Kirjeldus { get; set; } //optional sest index vaade ei vaja seda
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
