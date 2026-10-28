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

        //TDD esimene samm: lihtsad testid (1-3 kontrolli), mis kirjeldavad oodatud käitumist.
        //Osa neist on esialgu punased, sest teenus ei oska seda veel teha.

        //Create

        [Fact]
        public async Task Should_AddNewLecturer_WhenAllFieldsFilled()
        {
            //ülesseade
            LectureDTO dto = MockLecturerDtoData();

            //tegevus
            var result = await Svc<ILecturersServices>().Create(dto);

            //kontroll
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(dto.FirstName, result.FirstName);
        }

        [Fact]
        public async Task ShouldNot_AddNewLecturer_WhenNamesEmpty()
        {
            //ülesseade
            LectureDTO dto = MockLecturerDtoData();
            dto.FirstName = "";
            dto.LastName = "";

            //tegevus
            var result = await Svc<ILecturersServices>().Create(dto);

            //kontroll
            Assert.Null(result); //teenus lükkas lisamise tagasi
        }

        [Fact]
        public async Task ShouldNot_AddNewLecturer_WhenDtoIsNull()
        {
            //tegevus
            var result = await Svc<ILecturersServices>().Create(null!);

            //kontroll
            Assert.Null(result);
        }

        //Details

        [Fact]
        public async Task Should_ReturnLecturerDetails_WhenLecturerExists()
        {
            //ülesseade
            var created = await AddObjectToDB();

            //tegevus
            var result = await Svc<ILecturersServices>().DetailAsync(new LectureDTO { Id = created.Id });

            //kontroll
            Assert.NotNull(result);
            Assert.Equal(created.Id, result.Id);
        }

        [Fact]
        public async Task ShouldNot_ReturnLecturerDetails_WhenIdDoesNotExist()
        {
            //tegevus
            var result = await Svc<ILecturersServices>().DetailAsync(new LectureDTO { Id = Guid.NewGuid() });

            //kontroll
            Assert.Null(result);
        }

        //Update

        [Fact]
        public async Task Should_UpdateLecturer_WhenDataIsDifferentFromDB()
        {
            //ülesseade
            var created = await AddObjectToDB();
            LectureDTO update = MockLecturerDtoData();
            update.Id = created.Id;
            update.FirstName = "UusEesnimi";

            //tegevus
            var result = await Svc<ILecturersServices>().Update(update);

            //kontroll
            Assert.NotNull(result);
            Assert.Equal("UusEesnimi", result.FirstName);
        }

        [Fact]
        public async Task ShouldNot_UpdateLecturer_WhenLecturerDoesNotExist()
        {
            //ülesseade
            LectureDTO update = MockLecturerDtoData();
            update.Id = Guid.NewGuid();

            //tegevus
            var result = await Svc<ILecturersServices>().Update(update);

            //kontroll
            Assert.Null(result);
        }

        //Delete

        [Fact]
        public async Task Should_DeleteLecturer_WhenIdIsValid()
        {
            //ülesseade
            var created = await AddObjectToDB();

            //tegevus
            var result = await Svc<ILecturersServices>().Delete(new LectureDTO { Id = created.Id });
            var deleted = await Svc<ILecturersServices>().DetailAsync(new LectureDTO { Id = created.Id });

            //kontroll
            Assert.NotNull(result);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task ShouldNot_DeleteLecturer_WhenIdDoesNotExist()
        {
            //tegevus
            var result = await Svc<ILecturersServices>().Delete(new LectureDTO { Id = Guid.NewGuid() });

            //kontroll
            Assert.Null(result);
        }

        //getall

        [Fact]
        public async Task Should_ReturnAllLecturers_WhenLecturersExist()
        {
            //ülesseade
            await AddObjectToDB();
            await AddObjectToDB();

            //tegevus
            var result = await Svc<ILecturersServices>().GetAll();

            //kontroll
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
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
            Assert.Equal(4, result.Count);
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
