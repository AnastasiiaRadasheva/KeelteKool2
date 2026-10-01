using System;
using System.Collections.Generic;
using System.Text;

namespace KeeltekoolV2.Core.Domain
{
    //Domain kaustas olev mudel näitab ära täpse andmekuju millena kursuse andmed andmebaasis ka
    //hoitakse, siin võib samuti olla valikulisi andmeid, kuid tüüpiliselt on neid vähem
    //sest andmebaas hoiab tihtipeale ainult vajalikke andmeid.
    public class LanguageCourse
    {
        public Guid Id { get; set; }
        public string Nimetus { get; set; }
        public string Keel { get; set; }
        public string Tase { get; set; }
        public string Kirjeldus { get; set; }

        //Vajalikud andmeväljad, mida muudavad ainult kontroller ja/või service
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
