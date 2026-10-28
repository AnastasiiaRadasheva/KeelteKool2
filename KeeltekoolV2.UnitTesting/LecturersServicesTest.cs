using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using KeeltekoolV2.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KeeltekoolV2.UnitTesting
{
    public class LecturersServicesTest : TestBase
    {
        // [Fact] - käsusõna, mida testrunner tunneb, et aru saada mis on test, ja mis ei ole
        // Testi nimi koosneb kolmest osast:
        // 1 - kas test on tavaline (Should) või negatiivne (ShouldNot)
        // 2 - mida parasjagu testiga testitakse
        // 3 - mis tingimustel tulemust kontrollitakse

        //TDD kolmas samm: täiendatud testid. Iga test kontrollib vähemalt 5 asja (assert-i),
        //et oleks kindel, et teenus teeb täpselt seda, mida test väidab.
        //Testi kohal olev summary kirjeldab eesti keeles mida test tegema peab.

        //Create

        /// <summary>
        /// Test kontrollib, et kõigi väljadega täidetud andmete korral lisab teenus uue õppejõu andmebaasi:
        /// tagastatakse objekt uue (tühjast erineva) Id-ga, andmed on samad mis saadeti,
        /// loomise aeg on määratud ning andmebaasis on täpselt üks kirje.
        /// </summary>
        [Fact]
        public async Task Should_AddNewLecturer_WhenAllFieldsFilled()
        {
            //ülesseade
            LectureDTO dto = MockLecturerDtoData();

            //tegevus
            var result = await Svc<ILecturersServices>().Create(dto);

            //kontroll
            Assert.NotNull(result);                                      //teenus tagastas objekti
            Assert.NotEqual(Guid.Empty, result.Id);                      //objektile anti id
            Assert.Equal(dto.FirstName, result.FirstName);               //eesnimi on sama mis saadeti
            Assert.Equal(dto.LastName, result.LastName);                 //perenimi on sama mis saadeti
            Assert.Equal(dto.Qualifications, result.Qualifications);     //kvalifikatsioonid on samad mis saadeti
            Assert.NotEqual(default(DateTime), result.CreatedAt);        //loomise aeg määrati
            Assert.Equal(1, await CountLecturersInDb());                 //andmebaasis on täpselt üks õppejõud
        }

        /// <summary>
        /// Negatiivne test: kui eesnimi ja/või perenimi on tühi või koosneb ainult tühikutest,
        /// siis teenus õppejõudu ei lisa (tagastab null) ja andmebaasi ei teki ühtegi kirjet.
        /// </summary>
        [Fact]
        public async Task ShouldNot_AddNewLecturer_WhenNamesEmpty()
        {
            //ülesseade
            LectureDTO bothEmpty = MockLecturerDtoData();
            bothEmpty.FirstName = "";
            bothEmpty.LastName = "";

            LectureDTO firstNameEmpty = MockLecturerDtoData();
            firstNameEmpty.FirstName = "";

            LectureDTO lastNameEmpty = MockLecturerDtoData();
            lastNameEmpty.LastName = "";

            LectureDTO onlyWhitespace = MockLecturerDtoData();
            onlyWhitespace.FirstName = "   ";
            onlyWhitespace.LastName = "   ";

            //tegevus
            var result1 = await Svc<ILecturersServices>().Create(bothEmpty);
            var result2 = await Svc<ILecturersServices>().Create(firstNameEmpty);
            var result3 = await Svc<ILecturersServices>().Create(lastNameEmpty);
            var result4 = await Svc<ILecturersServices>().Create(onlyWhitespace);
            var all = await Svc<ILecturersServices>().GetAll();

            //kontroll
            Assert.Null(result1);                                        //mõlemad nimed tühjad
            Assert.Null(result2);                                        //ainult eesnimi tühi
            Assert.Null(result3);                                        //ainult perenimi tühi
            Assert.Null(result4);                                        //nimed koosnevad ainult tühikutest
            Assert.Empty(all);                                           //nimekiri on tühi
            Assert.Equal(0, await CountLecturersInDb());                 //andmebaasi ei lisatud midagi
        }

        /// <summary>
        /// Negatiivne test: kui teenusele antakse null (DTO puudub), siis tagastatakse null, viga ei teki
        /// ja andmebaasis juba olemas olev õppejõud jääb muutmata.
        /// </summary>
        [Fact]
        public async Task ShouldNot_AddNewLecturer_WhenDtoIsNull()
        {
            //ülesseade
            var existing = await AddObjectToDB();
            var existingId = existing.Id;
            var existingFirstName = existing.FirstName;

            //tegevus
            var result = await Svc<ILecturersServices>().Create(null!);
            var all = await Svc<ILecturersServices>().GetAll();

            //kontroll
            Assert.Null(result);                                         //teenus lükkas null-i tagasi
            Assert.Equal(1, await CountLecturersInDb());                 //kirjete arv ei muutunud
            var only = Assert.Single(all);                               //nimekirjas on endiselt ainult üks õppejõud
            Assert.Equal(existingId, only.Id);                           //see on sama õppejõud mis enne
            Assert.Equal(existingFirstName, only.FirstName);             //tema andmed ei muutunud
        }

        //Details

        /// <summary>
        /// Test kontrollib, et olemasoleva õppejõu Id järgi leiab DetailAsync õige õppejõu üles.
        /// Enne lugemist tühjendatakse konteksti mälu, et tulemus tuleks päriselt andmebaasist,
        /// ning kontrollitakse et kõik andmed on samad mis lisamisel.
        /// </summary>
        [Fact]
        public async Task Should_ReturnLecturerDetails_WhenLecturerExists()
        {
            //ülesseade
            LectureDTO dto = MockLecturerDtoData();
            var created = await Svc<ILecturersServices>().Create(dto);
            var id = created!.Id;
            ClearTracking();

            //tegevus
            var result = await Svc<ILecturersServices>().DetailAsync(new LectureDTO { Id = id });

            //kontroll
            Assert.NotNull(result);                                      //midagi leiti
            Assert.NotSame(created, result);                             //tegu on andmebaasist loetud uue objektiga
            Assert.Equal(id, result.Id);                                 //id on sama
            Assert.Equal(dto.FirstName, result.FirstName);               //eesnimi on sama
            Assert.Equal(dto.LastName, result.LastName);                 //perenimi on sama
            Assert.Equal(dto.Qualifications, result.Qualifications);     //kvalifikatsioonid on samad
            Assert.Equal(created.CreatedAt, result.CreatedAt);           //loomise aeg on sama
        }

        /// <summary>
        /// Negatiivne test: kui Id-d andmebaasis ei ole, Id on tühi (Guid.Empty) või DTO on null,
        /// siis tagastatakse null. Samal ajal peab olemasolev õppejõud endiselt leitav olema.
        /// </summary>
        [Fact]
        public async Task ShouldNot_ReturnLecturerDetails_WhenIdDoesNotExist()
        {
            //ülesseade
            var existing = await AddObjectToDB();

            //tegevus
            var unknownId = await Svc<ILecturersServices>().DetailAsync(new LectureDTO { Id = Guid.NewGuid() });
            var emptyId = await Svc<ILecturersServices>().DetailAsync(new LectureDTO { Id = Guid.Empty });
            var nullDto = await Svc<ILecturersServices>().DetailAsync(null!);
            var stillThere = await Svc<ILecturersServices>().DetailAsync(new LectureDTO { Id = existing.Id });

            //kontroll
            Assert.Null(unknownId);                                      //tundmatu id
            Assert.Null(emptyId);                                        //tühi id
            Assert.Null(nullDto);                                        //DTO puudub
            Assert.NotNull(stillThere);                                  //olemasolev õppejõud on endiselt leitav
            Assert.Equal(existing.Id, stillThere.Id);                    //ja see on õige õppejõud
        }

        //Update

        /// <summary>
        /// Test kontrollib, et õppejõu andmete muutmisel salvestuvad uued andmed: uuendatud objekt
        /// ja andmebaasist uuesti loetud kirje sisaldavad uusi nimesid ja kvalifikatsioone,
        /// õppejõudude arv ei muutu (ei tekkinud uut kirjet).
        /// </summary>
        [Fact]
        public async Task Should_UpdateLecturer_WhenDataIsDifferentFromDB()
        {
            //ülesseade
            var created = await AddObjectToDB();
            var id = created.Id;

            LectureDTO update = new LectureDTO
            {
                Id = id,
                FirstName = "UusEesnimi",
                LastName = "UusPerenimi",
                Qualifications = "PhD keeleteaduses"
            };

            //tegevus
            var result = await Svc<ILecturersServices>().Update(update);
            ClearTracking();
            var fromDb = await Svc<ILecturersServices>().DetailAsync(new LectureDTO { Id = id });

            //kontroll
            Assert.NotNull(result);                                      //teenus tagastas objekti
            Assert.Equal(id, result.Id);                                 //id jäi samaks
            Assert.Equal(update.FirstName, result.FirstName);            //tagastatud eesnimi on uus
            Assert.Equal(update.LastName, result.LastName);              //tagastatud perenimi on uus
            Assert.NotNull(fromDb);                                      //andmebaasist leiti kirje
            Assert.Equal(update.FirstName, fromDb.FirstName);            //andmebaasis on uus eesnimi
            Assert.Equal(update.LastName, fromDb.LastName);              //andmebaasis on uus perenimi
            Assert.Equal(update.Qualifications, fromDb.Qualifications);  //andmebaasis on uued kvalifikatsioonid
            Assert.Equal(1, await CountLecturersInDb());                 //uut kirjet ei tekkinud
        }

        /// <summary>
        /// Negatiivne test: olematu Id või null DTO korral tagastatakse null, uut õppejõudu juurde ei looda
        /// ja olemasoleva õppejõu andmed jäävad muutmata.
        /// </summary>
        [Fact]
        public async Task ShouldNot_UpdateLecturer_WhenLecturerDoesNotExist()
        {
            //ülesseade
            var existing = await AddObjectToDB();
            var existingId = existing.Id;
            var existingFirstName = existing.FirstName;

            LectureDTO update = MockLecturerDtoData();
            update.Id = Guid.NewGuid();
            update.FirstName = "Muudetud";

            //tegevus
            var resultUnknownId = await Svc<ILecturersServices>().Update(update);
            var resultNullDto = await Svc<ILecturersServices>().Update(null!);
            var phantom = await Svc<ILecturersServices>().DetailAsync(new LectureDTO { Id = update.Id });
            ClearTracking();
            var existingFromDb = await Svc<ILecturersServices>().DetailAsync(new LectureDTO { Id = existingId });

            //kontroll
            Assert.Null(resultUnknownId);                                //tundmatu id
            Assert.Null(resultNullDto);                                  //DTO puudub
            Assert.Null(phantom);                                        //olematu id alla ei tekkinud uut kirjet
            Assert.NotNull(existingFromDb);                              //olemasolev õppejõud on alles
            Assert.Equal(existingFirstName, existingFromDb.FirstName);   //ja tema andmed ei muutunud
            Assert.Equal(1, await CountLecturersInDb());                 //kirjete arv ei muutunud
        }

        //Delete

        /// <summary>
        /// Test kontrollib, et kehtiva Id korral kustutatakse õppejõud andmebaasist: Delete tagastab
        /// kustutatud õppejõu andmed, DetailAsync ei leia teda enam ja andmebaasis ei ole ühtegi kirjet.
        /// </summary>
        [Fact]
        public async Task Should_DeleteLecturer_WhenIdIsValid()
        {
            //ülesseade
            var created = await AddObjectToDB();
            var id = created.Id;
            var firstName = created.FirstName;
            var lastName = created.LastName;
            var qualifications = created.Qualifications;

            //tegevus
            var result = await Svc<ILecturersServices>().Delete(new LectureDTO { Id = id });
            var deleted = await Svc<ILecturersServices>().DetailAsync(new LectureDTO { Id = id });

            //kontroll
            Assert.NotNull(result);                                      //delete tagastas kustutatud objekti
            Assert.Equal(id, result.Id);                                 //kustutati õige õppejõud
            Assert.Equal(firstName, result.FirstName);                   //eesnimi on sama
            Assert.Equal(lastName, result.LastName);                     //perenimi on sama
            Assert.Equal(qualifications, result.Qualifications);         //kvalifikatsioonid on samad
            Assert.Null(deleted);                                        //andmebaasist ei leitud enam objekti
            Assert.Equal(0, await CountLecturersInDb());                 //andmebaas on tühi
        }

        /// <summary>
        /// Negatiivne test: olematu Id, tühi Id või null DTO korral tagastatakse null
        /// ja olemasolev õppejõud jääb andmebaasi alles.
        /// </summary>
        [Fact]
        public async Task ShouldNot_DeleteLecturer_WhenIdDoesNotExist()
        {
            //ülesseade
            var existing = await AddObjectToDB();

            //tegevus
            var unknownId = await Svc<ILecturersServices>().Delete(new LectureDTO { Id = Guid.NewGuid() });
            var emptyId = await Svc<ILecturersServices>().Delete(new LectureDTO { Id = Guid.Empty });
            var nullDto = await Svc<ILecturersServices>().Delete(null!);
            var stillThere = await Svc<ILecturersServices>().DetailAsync(new LectureDTO { Id = existing.Id });

            //kontroll
            Assert.Null(unknownId);                                      //tundmatu id
            Assert.Null(emptyId);                                        //tühi id
            Assert.Null(nullDto);                                        //DTO puudub
            Assert.NotNull(stillThere);                                  //olemasolev õppejõud on alles
            Assert.Equal(1, await CountLecturersInDb());                 //kirjete arv ei muutunud
        }

        //getall

        /// <summary>
        /// Test kontrollib, et GetAll tagastab kõik andmebaasis olevad õppejõud:
        /// nimekirja pikkus on õige, iga lisatud õppejõud on nimekirjas olemas ja kõik Id-d on omavahel erinevad.
        /// </summary>
        [Fact]
        public async Task Should_ReturnAllLecturers_WhenLecturersExist()
        {
            //ülesseade
            var first = await Svc<ILecturersServices>().Create(new LectureDTO { FirstName = "Adriana", LastName = "Pikaljov", Qualifications = "Loll" });
            var second = await Svc<ILecturersServices>().Create(new LectureDTO { FirstName = "Anastasiia", LastName = "Radasheva", Qualifications = "MA" });
            var third = await Svc<ILecturersServices>().Create(new LectureDTO { FirstName = "Mari", LastName = "Speek", Qualifications = "BA" });

            //tegevus
            var result = await Svc<ILecturersServices>().GetAll();

            //kontroll
            Assert.NotNull(result);                                              //tagastati nimekiri
            Assert.Equal(3, result.Count);                                       //nimekirjas on kolm õppejõudu
            Assert.Contains(result, x => x.Id == first!.Id);                     //esimene on nimekirjas
            Assert.Contains(result, x => x.Id == second!.Id);                    //teine on nimekirjas
            Assert.Contains(result, x => x.Id == third!.Id);                     //kolmas on nimekirjas
            Assert.Equal(3, result.Select(x => x.Id).Distinct().Count());       //kõik id-d on erinevad
        }

        //enda välja mõeldud testid

        /// <summary>
        /// Test kontrollib, et õppejõu lisamisel eemaldatakse eesnime, perenime ja kvalifikatsioonide
        /// algusest ning lõpust tühikud. Kontrollitakse nii teenuse tagastatud objekti kui ka
        /// andmebaasist uuesti loetud kirjet ning seda, et andmebaasi tekkis täpselt üks kirje.
        /// </summary>
        [Fact]
        public async Task Should_TrimNames_WhenNamesHaveWhitespace()
        {
            //ülesseade
            LectureDTO dto = new LectureDTO
            {
                FirstName = "  Adriana  ",
                LastName = "  Pikaljov  ",
                Qualifications = "  loll keeleteaduses  "
            };

            //tegevus
            var result = await Svc<ILecturersServices>().Create(dto);
            ClearTracking(); //sunnime järgmise lugemise päriselt andmebaasist tulema
            var fromDb = await Svc<ILecturersServices>().DetailAsync(new LectureDTO { Id = result?.Id ?? Guid.Empty });

            //kontroll
            Assert.NotNull(result);
            Assert.Equal("Adriana", result.FirstName);
            Assert.Equal("Pikaljov", result.LastName);
            Assert.Equal("loll keeleteaduses", result.Qualifications);
            Assert.NotNull(fromDb);
            Assert.Equal("Adriana", fromDb.FirstName);
            Assert.Equal(1, await CountLecturersInDb());
        }

        /// <summary>
        /// Test kontrollib, et kui õppejõule antakse kaasa kasutajakonto id (UserID), siis see salvestatakse,
        /// ning kui seda ei anta, siis jääb UserID tühjaks (null). Kontrollitakse ka seda, et andmebaasis
        /// on peale kahe lisamise täpselt kaks kirjet.
        /// </summary>
        [Fact]
        public async Task Should_SaveUserId_WhenUserIdIsGiven()
        {
            //ülesseade
            LectureDTO withUser = MockLecturerDtoData();
            withUser.UserID = "kasutaja-id-123";
            LectureDTO withoutUser = MockLecturerDtoData();
            withoutUser.UserID = null;

            //tegevus
            var created1 = await Svc<ILecturersServices>().Create(withUser);
            var created2 = await Svc<ILecturersServices>().Create(withoutUser);
            ClearTracking();
            var fromDb = await Svc<ILecturersServices>().DetailAsync(new LectureDTO { Id = created1?.Id ?? Guid.Empty });

            //kontroll
            Assert.NotNull(created1);
            Assert.Equal("kasutaja-id-123", created1.UserID);
            Assert.NotNull(fromDb);
            Assert.Equal("kasutaja-id-123", fromDb.UserID);
            Assert.NotNull(created2);
            Assert.Null(created2.UserID);
            Assert.Equal(2, await CountLecturersInDb());
        }

        /// <summary>
        /// Negatiivne test: kui muutmisel on eesnimi või perenimi tühi, siis uuendamist ei tehta (tagastatakse null)
        /// ja andmebaasis olevad andmed (sh kvalifikatsioonid ja muutmise aeg) jäävad täpselt samaks,
        /// isegi kui samal ajal üritati muuta ka kvalifikatsioone.
        /// </summary>
        [Fact]
        public async Task ShouldNot_ChangeData_WhenUpdateNamesEmpty()
        {
            //ülesseade
            var created = await AddObjectToDB();
            var id = created.Id;
            var firstName = created.FirstName;
            var lastName = created.LastName;
            var qualifications = created.Qualifications;
            var modifiedAt = created.ModifiedAt;

            LectureDTO update = new LectureDTO
            {
                Id = id,
                FirstName = "",
                LastName = "UusPerenimi",
                Qualifications = "Muudetud kvalifikatsioon"
            };

            //tegevus
            var resultEmptyFirstName = await Svc<ILecturersServices>().Update(update);
            update.FirstName = "UusEesnimi";
            update.LastName = "   ";
            var resultBlankLastName = await Svc<ILecturersServices>().Update(update);
            ClearTracking();
            var fromDb = await Svc<ILecturersServices>().DetailAsync(new LectureDTO { Id = id });

            //kontroll
            Assert.Null(resultEmptyFirstName);
            Assert.Null(resultBlankLastName);
            Assert.NotNull(fromDb);
            Assert.Equal(firstName, fromDb.FirstName);
            Assert.Equal(lastName, fromDb.LastName);
            Assert.Equal(qualifications, fromDb.Qualifications);
            Assert.Equal(modifiedAt, fromDb.ModifiedAt);
        }

        /// <summary>
        /// Test kontrollib ajatemplite õigsust muutmisel: loomise aeg (CreatedAt) ja Id jäävad samaks,
        /// aga muutmise aeg (ModifiedAt) uueneb hilisemaks. Sama kontrollitakse ka andmebaasist uuesti loetud kirjel.
        /// </summary>
        [Fact]
        public async Task Should_KeepCreatedAt_AndRefreshModifiedAt_WhenUpdated()
        {
            //ülesseade
            var created = await AddObjectToDB();
            var id = created.Id;
            var createdAt = created.CreatedAt;
            var modifiedAtBefore = created.ModifiedAt;

            LectureDTO update = MockLecturerDtoData();
            update.Id = id;
            update.FirstName = "Muudetud";

            await Task.Delay(50); //väike paus, et uus kellaaeg oleks kindlasti hilisem

            //tegevus
            var result = await Svc<ILecturersServices>().Update(update);
            ClearTracking();
            var fromDb = await Svc<ILecturersServices>().DetailAsync(new LectureDTO { Id = id });

            //kontroll
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(createdAt, result.CreatedAt);
            Assert.True(result.ModifiedAt > modifiedAtBefore);
            Assert.NotNull(fromDb);
            Assert.Equal(createdAt, fromDb.CreatedAt);
            Assert.Equal(result.ModifiedAt, fromDb.ModifiedAt);
        }

        /// <summary>
        /// Test kontrollib, et GetAll tagastab õppejõud järjestatuna: esmalt perenime järgi tähestikuliselt
        /// ning sama perenime korral eesnime järgi, sõltumata sellest millises järjekorras need lisati.
        /// </summary>
        [Fact]
        public async Task Should_ReturnLecturersSortedByLastNameThenFirstName_WhenGetAll()
        {
            //ülesseade - tahtlikult segamini järjekorras
            await Svc<ILecturersServices>().Create(new LectureDTO { FirstName = "Anastasiia", LastName = "Radasheva", Qualifications = "" });
            await Svc<ILecturersServices>().Create(new LectureDTO { FirstName = "Oleksandra", LastName = "Ryshniak", Qualifications = "" });
            await Svc<ILecturersServices>().Create(new LectureDTO { FirstName = "Marina", LastName = "Oleinik", Qualifications = "" });
            await Svc<ILecturersServices>().Create(new LectureDTO { FirstName = "Mari", LastName = "Speek", Qualifications = "" });

            //tegevus
            var result = await Svc<ILecturersServices>().GetAll();

            //kontroll
            Assert.NotNull(result);
            Assert.Equal("Radasheva Anastasiia", result[0].LastName + " " + result[0].FirstName);
            Assert.Equal("Ryshniak Oleksandra", result[1].LastName + " " + result[1].FirstName);
            Assert.Equal("Oleinik Marina", result[2].LastName + " " + result[2].FirstName);
            Assert.Equal("Speek Mari", result[3].LastName + " " + result[3].FirstName);
        }

        //abimeetodid

        //loeb andmebaasist kokku kui palju õppejõude seal päriselt on
        private async Task<int> CountLecturersInDb()
        {
            return await Svc<KeeltekoolV2Context>().Lecturer.CountAsync();
        }

        //unustab kõik andmebaasikonteksti mällu salvestatud objektid, nii tuleb järgmine lugemine päriselt andmebaasist
        private void ClearTracking()
        {
            Svc<KeeltekoolV2Context>().ChangeTracker.Clear();
        }

        private async Task<Lecturer> AddObjectToDB()
        {
            LectureDTO dto = MockLecturerDtoData();
            var created = await Svc<ILecturersServices>().Create(dto);
            return created!;
        }

        private LectureDTO MockLecturerDtoData()
        {
            return new LectureDTO
            {
                FirstName = "Adriana",
                LastName = "Pikaljov",
                Qualifications = "Loll keeleteaduses"
            };
        }
    }
}
