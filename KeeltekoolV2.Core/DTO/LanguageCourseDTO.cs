using System;
using System.Collections.Generic;
using System.Text;

namespace KeeltekoolV2.Core.DTO
{
    //Andmevahendusobjekt, ei pea vastama andmebaasis nõutud andmetele,
    //selle eesmärk on frontendi kontrolleri ja backendi teenuse vahel
    //andmete üle andmine, osad andmed võivad olla valikulised (märgitud "?" märgiga),
    //kuna service või kontroller saab omalt poolt midagi vajadusel muuta
    // või juurde lisada millel lõppkasutajal juurdepääsu olla ei tohiks.
    public class LanguageCourseDTO
    {
        public Guid? Id { get; set; }
        public string Nimetus { get; set; }
        public string Keel { get; set; }
        public string? Tase { get; set; }
        public string? Kirjeldus { get; set; }

        //Vajalikud andmeväljad, mida muudavad ainult kontroller ja/või service
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
