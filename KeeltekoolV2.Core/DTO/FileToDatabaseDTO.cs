using System.ComponentModel.DataAnnotations;

namespace KeeltekoolV2.Core.DTO
{
    public class FileToDatabaseDTO
    {
        [Key]

        public Guid ImageID { get; set; }

        public string? ImageTitle { get; set; }

        public byte[]? ImageData { get; set; }

        public Guid? LecturerId { get; set; }
    }
}