using System;
using System.Collections.Generic;
using System.Text;

namespace KeeltekoolV2.Core.Domain
{
    //Domain kaustas olev mudel näitab ära täpse andmekuju millena õpilase andmed andmebaasis
    //hoitakse. Tavakasutaja võib olla kas Lecturer või Student, seega on Student eraldi tabel.
    public class Student
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string LanguageLevel { get; set; }
        public string? UserID { get; set; }

        //Vajalikud andmeväljad, mida muudavad ainult kontroller ja/või service
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
