using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KeeltekoolV2.Core.Domain
{
    public class FileToDatabase
    {
        [Key]

        public Guid ImageID { get; set; }
 
        public string? ImageTitle { get; set; }
       
        public byte[]? ImageData { get; set; }

        public Guid? LecturerId { get; set; }
    }
}

