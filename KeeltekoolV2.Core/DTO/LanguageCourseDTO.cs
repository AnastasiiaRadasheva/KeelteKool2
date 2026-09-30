using System;
using System.Collections.Generic;
using System.Text;

namespace KeeltekoolV2.Core.DTO
{
    //Andmevahendusobjekt, ei pea vastama andmebaasis nõutud andmetele,
    //selle eesmärk on frontendi kontrolleri ja backend teenuse vahel
    //andmete üle andmine osd andmed võivad olla valikulised (?)
    //kuna sevise või kontroller saab omalt poolt midagi vajadusel muuta 
    //või juurde lisada millel lõppkasutajal juurderpääsu olla ei tohiks
    public class LanguageCourseDTO
    {
        public Guid Id { get; set; }
        public string Nimetus { get; set; }
        public string Keel { get; set; }
        public string? Tase { get; set; }
        public string? Kirjeldus { get; set; }
    }



}
