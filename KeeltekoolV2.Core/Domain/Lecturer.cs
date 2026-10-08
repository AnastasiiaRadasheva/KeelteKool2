using KeeltekoolV2.Core.DTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeeltekoolV2.Core.Domain
{
    public class Lecturer
    {
        public  Guid Id{ get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }



        public string Qualifications { get; set; }
        public string UserID { get; set; }
        public IEnumerable<FileToDatabaseDTO> Image { get; set; } = new List<FileToDatabaseDTO>();

        //   public ICollection<Subject> LanguageSubject { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
