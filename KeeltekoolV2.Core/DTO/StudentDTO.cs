using System;
using System.Collections.Generic;
using System.Text;

namespace KeeltekoolV2.Core.DTO
{
    //Andmevahendusobjekt frontendi kontrolleri ja backendi teenuse vahel,
    //osad andmed võivad olla valikulised (märgitud "?" märgiga).
    public class StudentDTO
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? LanguageLevel { get; set; }
        public string? UserID { get; set; }

        //Vajalikud andmeväljad, mida muudavad ainult kontroller ja/või service
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
