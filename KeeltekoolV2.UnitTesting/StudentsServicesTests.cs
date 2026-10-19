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
        /// <summary>
        /// Test kontrollib, et uue õpilase lisamisel tagastab teenus objekti.
        /// </summary>
        [Fact]
        public async Task Should_AddNewStudent_WhenResultIsReturned()
        {
            //ülesseade
            StudentDTO dto = new StudentDTO();
            dto.FirstName = "Test";
            dto.LastName = "Test";
            dto.Email = "testanastasiia@test.ee";
            dto.LanguageLevel = "A1";

            //tegevus
            var result = await Svc<IStudentsServices>().Create(dto);

            //kontroll
            Assert.NotNull(result);
        }

        /// <summary>
        /// Test kontrollib, et lisatud õpilase andmed saab id järgi andmebaasist kätte.
        /// </summary>
        [Fact]
        public async Task Should_GetStudentDetails_WhenGuidIsNotNull()
        {
            //ülesseade
            StudentDTO dto = new StudentDTO();
            dto.FirstName = "Test";
            dto.LastName = "Test";
            dto.Email = "testanastasiia@test.ee";
            dto.LanguageLevel = "A1";
            var createdStudent = await Svc<IStudentsServices>().Create(dto);

            //tegevus
            var result = await Svc<IStudentsServices>().DetailsAsync(createdStudent.Id);

            //kontroll
            Assert.NotNull(result);
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
    }
}
