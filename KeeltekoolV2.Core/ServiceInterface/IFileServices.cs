using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.DTO;

namespace KeeltekoolV2.Core.ServiceInterface
{
    public interface IFileServices
    {
        void UploadFilesToDatabase(LectureDTO dto, Lecturer domain);

        Task<FileToDatabase> RemoveFileFromDatabase(FileToDatabaseDTO dto);

        Task<FileToDatabase> RemoveFileFromDatabase(FileToDatabaseDTO[] dto);
    }
}