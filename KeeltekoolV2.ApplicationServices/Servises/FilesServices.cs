using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using KeeltekoolV2.Data;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeeltekoolV2.ApplicationServices.Servises
{
    public class FilesServices : IFileServices
    {
        private readonly KeeltekoolV2Context _context;
        public FilesServices(KeeltekoolV2Context context)
        {
            _context = context;
        }
        public Task<FileToDatabase> RemoveFileFromDatabase(FileToDatabaseDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<FileToDatabase> RemoveFileFromDatabase(FileToDatabaseDTO[] dto)
        {
            throw new NotImplementedException();
        }

        public void UploadFilesToDatabase(LectureDTO dto, Lecturer domain)
        {
            if (dto.Files != null && dto.Files.Count > 0)
            {
                foreach (var file in dto.Files)
                {
                    using (var target = new MemoryStream())
                    {
                        FileToDatabase files = new FileToDatabase()
                        {
                            ImageID = Guid.NewGuid(),
                            ImageTitle = file.FileName,
                            LecturerId = domain.Id
                        };

                        file.CopyTo(target);
                        files.ImageData = target.ToArray();

                        _context.Files.Add(files);
                    }
                }
            }
        }
    }
}
