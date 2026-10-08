using Microsoft.AspNetCore.Http;

namespace KeeltekoolV2.Core.DTO
{
    public class LectureDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Fullname TBA
        public string Qualifications { get; set; }
        public string? UserID { get; set; }
        //public ICollection<LanguageSubject> LanguageSubjects { get; set; }

        public List<IFormFile>? Files { get; set; }
        public IEnumerable<FileToDatabaseDTO>? Image { get; set; } = new List<FileToDatabaseDTO>();

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}