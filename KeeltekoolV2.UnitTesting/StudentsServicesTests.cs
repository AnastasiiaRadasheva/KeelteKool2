using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
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
        /// Test kontrollib, et olemasoleva õpilase andmeid saab uute andmetega muuta.
        /// </summary>
        [Fact]
        public async Task Should_UpdateStudentWithNewData_WhenDataIsDifferentFromDB()
        {
            //ülesseade
            StudentDTO dto = new StudentDTO();
            dto.FirstName = "Test";
            dto.LastName = "Test";
            dto.Email = "test@test.ee";
            dto.LanguageLevel = "A1";
            var createdStudent = await Svc<IStudentsServices>().Create(dto);

            StudentDTO updatedInfo = new StudentDTO();
            updatedInfo.Id = createdStudent.Id;
            updatedInfo.FirstName = "TestUusInfo";
            updatedInfo.LastName = "TestUusPerenimi";
            updatedInfo.Email = "uus@test.ee";
            updatedInfo.LanguageLevel = "B2";

            //tegevus
            var result = await Svc<IStudentsServices>().Update(updatedInfo);

            //kontroll
            Assert.NotNull(result);
        }

        /// <summary>
        /// Test kontrollib, et õpilase saab kehtiva id järgi kustutada bebebebe
        /// </summary>
        [Fact]
        public async Task Should_DeleteStudent_WhenValidIdIsGiven()
        {
            //ülesseade
            StudentDTO dto = new StudentDTO();
            dto.FirstName = "Test";
            dto.LastName = "Test";
            dto.Email = "testanastasiia@test.ee";
            dto.LanguageLevel = "A1";
            var createdStudent = await Svc<IStudentsServices>().Create(dto);

            //tegevus
            var result = await Svc<IStudentsServices>().Delete(createdStudent.Id);

            //kontroll
            Assert.NotNull(result);
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
