using Microsoft.AspNetCore.Mvc;

namespace KeelteKool2.Models.LanguageCourses
{
    //ViewModel on vajalik kasutajale info kuvamiseks, ja sealt edasi controllerile andmiseks, Viewmodel
    //erineb DTO-objektist selle võrra, et kõik kasutajale mittevajalikud andmed on sealt eemaldatud.
    //Valikulised andmed mis kuuluvad ka ka hiljem kasutajatele esitamiseks siiski jäävad.
    //See eraldatus tagab ka selle et kasutaja ei saa pahatahtlikult soovimatutele andmetele ligipääsu. The End
    //and they all lived happily ever after
    public class LanguageCourseViewModel : Controller
    {
        public Guid Id { get; set; }
        public string Nimetus { get; set; }
        public string Keel { get; set; }
        public string Tase { get; set; }
        public string Kirjeldus { get; set; }
    }
}
