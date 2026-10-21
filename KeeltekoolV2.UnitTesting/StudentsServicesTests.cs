using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using KeeltekoolV2.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace KeeltekoolV2.UnitTesting
{
    public class StudentsServicesTests : TestBase
    {
        // Testi nimi koosneb kolmest osast:
        // 1 - kas test on tavaline (Should) või negatiivne (ShouldNot)
        // 2 - mida parasjagu testiga testitakse
        // 3 - mis tingimustel tulemust kontrollitakse

        /// <summary>
        /// Test kontrollib, et kui kõik väljad on täidetud, lisab teenus uue õpilase andmebaasi
        /// ning tagastatud objektil on uus id, saadetud andmed ja loomise aeg.
        /// </summary>
        [Fact]
        public async Task Should_AddNewStudent_WhenResultIsReturned()
        {
            //ülesseade
            StudentDTO dto = MockStudentDtoData();

            //tegevus
            var result = await Svc<IStudentsServices>().Create(dto);

            //kontroll
            Assert.NotNull(result);                                  //teenus tagastas objekti
            Assert.NotEqual(Guid.Empty, result.Id);                  //objektile anti id
            Assert.Equal(dto.FirstName, result.FirstName);           //eesnimi on sama mis saadeti
            Assert.Equal(dto.LastName, result.LastName);             //perenimi on sama mis saadeti
            Assert.Equal(dto.Email, result.Email);                   //e-post on sama mis saadeti
            Assert.Equal(dto.LanguageLevel, result.LanguageLevel);   //keeletase on sama mis saadeti
            Assert.NotEqual(default, result.CreatedAt);              //loomise aeg on määratud
        }

        /// <summary>
        /// Test kontrollib, et andmebaasi lisatud õpilase saab id järgi tagasi lugeda
        /// ning loetud andmed on samad, mis lisamisel.
        /// </summary>
        [Fact]
        public async Task Should_GetStudentDetails_WhenGuidIsNotNull()
        {
            //ülesseade
            Student createdStudent = await AddObjectToDB();

            //tegevus
            var result = await Svc<IStudentsServices>().DetailsAsync(createdStudent.Id);

            //kontroll
            Assert.NotNull(result);                                              //tagastati objekt
            Assert.Equal(createdStudent.Id, result.Id);                          //id on sama mis lisatud objektil
            Assert.Equal(createdStudent.FirstName, result.FirstName);            //eesnimi on sama
            Assert.Equal(createdStudent.LastName, result.LastName);              //perenimi on sama
            Assert.Equal(createdStudent.Email, result.Email);                    //e-post on sama
            Assert.Equal(createdStudent.LanguageLevel, result.LanguageLevel);    //keeletase on sama
        }

        /// <summary>
        /// Test kontrollib, et olemasoleva õpilase andmeid saab uute andmetega muuta:
        /// andmebaasis on uued andmed, id ja loomise aeg jäävad samaks, muutmise aeg uueneb.
        /// </summary>
        [Fact]
        public async Task Should_UpdateStudentWithNewData_WhenDataIsDifferentFromDB()
        {
            //ülesseade
            Student createdStudent = await AddObjectToDB();
            //jätame algsed väärtused meelde, sest teenus muudab sama objekti
            DateTime originalCreatedAt = createdStudent.CreatedAt;
            DateTime originalModifiedAt = createdStudent.ModifiedAt;

            StudentDTO updatedInfo = new StudentDTO();
            updatedInfo.Id = createdStudent.Id;
            updatedInfo.FirstName = "TestUusInfo";
            updatedInfo.LastName = "TestUusPerenimi";
            updatedInfo.Email = "uus@test.ee";
            updatedInfo.LanguageLevel = "B2";

            //tegevus
            await Svc<IStudentsServices>().Update(updatedInfo);

            //kontroll
            var result = await Svc<IStudentsServices>().DetailsAsync(createdStudent.Id);

            Assert.NotNull(result);                                          //õpilane on endiselt andmebaasis
            Assert.Equal(updatedInfo.Id, result.Id);                         //id ei muutunud
            Assert.Equal(updatedInfo.FirstName, result.FirstName);           //eesnimi muudeti
            Assert.Equal(updatedInfo.LastName, result.LastName);             //perenimi muudeti
            Assert.Equal(updatedInfo.Email, result.Email);                   //e-post muudeti
            Assert.Equal(updatedInfo.LanguageLevel, result.LanguageLevel);   //keeletase muudeti
            Assert.Equal(originalCreatedAt, result.CreatedAt);               //loomise aeg jäi samaks
            Assert.True(result.ModifiedAt >= originalModifiedAt);            //muutmise aeg uuenes
        }

        /// <summary>
        /// Test kontrollib, et õpilase saab kehtiva id järgi kustutada:
        /// teenus tagastab kustutatud objekti ja andmebaasist seda enam ei leita.
        /// </summary>
        [Fact]
        public async Task Should_DeleteStudent_WhenValidIdIsGiven()
        {
            //ülesseade
            Student createdStudent = await AddObjectToDB();

            //tegevus
            var result = await Svc<IStudentsServices>().Delete(createdStudent.Id);
            var deleted = await Svc<IStudentsServices>().DetailsAsync(createdStudent.Id);

            //kontroll
            Assert.Null(deleted);                                            //andmebaasist ei leitud objekti
            Assert.NotNull(result);                                          //delete tagastas kustutatud objekti
            Assert.Equal(createdStudent.Id, result.Id);                      //kustutati õige õpilane
            Assert.Equal(createdStudent.FirstName, result.FirstName);
            Assert.Equal(createdStudent.LastName, result.LastName);
            Assert.Equal(createdStudent.Email, result.Email);
        }

        // ---------------- ise välja mõeldud testid ----------------

        /// <summary>
        /// Negatiivne test: kui ees- ja perenimi on tühjad, ei tohi teenus õpilast lisada
        /// ning andmebaasi ei teki ühtegi uut kirjet.
        /// </summary>
        [Fact]
        public async Task ShouldNot_AddStudent_WhenNamesAreEmpty()
        {
            //ülesseade
            StudentDTO dto = MockStudentDtoData();
            dto.FirstName = "";
            dto.LastName = " ";
            dto.Email = "tyhjadnimed@test.ee";
            var context = Svc<KeeltekoolV2Context>();
            int countBefore = context.Students.Count();

            //tegevus
            var result = await Svc<IStudentsServices>().Create(dto);

            //kontroll
            int countAfter = context.Students.Count();
            Assert.Null(result);                                                     //teenus lükkas lisamise tagasi
            Assert.Equal(countBefore, countAfter);                                   //andmebaasi ei lisatud midagi
            Assert.False(context.Students.Any(x => x.Email == dto.Email));           //sellise e-postiga õpilast ei ole
            Assert.True(string.IsNullOrWhiteSpace(dto.FirstName));                   //eesnimi oli tõesti tühi
            Assert.True(string.IsNullOrWhiteSpace(dto.LastName));                    //perenimi oli tõesti tühi
        }

        /// <summary>
        /// Negatiivne test: kui e-posti aadressis puudub @ märk, ei tohi teenus õpilast lisada.
        /// </summary>
        [Fact]
        public async Task ShouldNot_AddStudent_WhenEmailIsInvalid()
        {
            //ülesseade
            StudentDTO dto = MockStudentDtoData();
            dto.FirstName = "Vale";
            dto.LastName = "Email";
            dto.Email = "vale.email.test.ee";
            var context = Svc<KeeltekoolV2Context>();
            int countBefore = context.Students.Count();

            //tegevus
            var result = await Svc<IStudentsServices>().Create(dto);

            //kontroll
            int countAfter = context.Students.Count();
            Assert.Null(result);                                                     //teenus lükkas lisamise tagasi
            Assert.Equal(countBefore, countAfter);                                   //andmebaasi ei lisatud midagi
            Assert.False(context.Students.Any(x => x.Email == dto.Email));           //vale e-postiga kirjet ei ole
            Assert.False(context.Students.Any(x => x.FirstName == "Vale" && x.LastName == "Email"));
            Assert.DoesNotContain("@", dto.Email);                                   //e-post oli tõesti ilma @ märgita
        }

        //abimeetod: lisab andmebaasi testõpilase ja tagastab selle
        private async Task<Student> AddObjectToDB()
        {
            StudentDTO dto = MockStudentDtoData();
            var created = await Svc<IStudentsServices>().Create(dto);
            return created;
        }

        //abimeetod: tekitab testandmetega StudentDTO
        private StudentDTO MockStudentDtoData()
        {
            return new StudentDTO
            {
                FirstName = "Test",
                LastName = "Test",
                Email = "testanastasiia@test.ee",
                LanguageLevel = "A1"
            };
        }
    }
}
