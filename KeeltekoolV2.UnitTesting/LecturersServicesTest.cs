using KeeltekoolV2.ApplicationServices.Servises;
using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KeeltekoolV2.UnitTesting
{
    public class LecturersServicesTest : TestBase
    {
        [Fact]
        public async Task Should_AddnewLecture_WhenResultISReturned()
        {
            //ülesseade
            LectureDTO dto = new LectureDTO();
            dto.FirstName = "TestFnameCreate";
            dto.LastName = "TestlNameCreate";
            dto.Qualifications = "TestQualificationsCreate";

            var result = await Svc<ILecturersServices>().Create(dto);

            Assert.NotNull(result);
        }



        //details test
        [Fact]
        public async Task Should_GetLecturerDetails_WhenLecturerExists()
        {
            //ülesseade
            LectureDTO dto = new LectureDTO();
            dto.FirstName = "TestDelete";
            dto.LastName = "TestDelete";
            dto.Qualifications = "TestDelete";

            var created = await Svc<ILecturersServices>().Create(dto);

            //tegevus
            LectureDTO searchDto = new LectureDTO();
            searchDto.Id = created.Id;
            var result = await Svc<ILecturersServices>().DetailAsync(searchDto);

            //kontroll
            Assert.NotNull(result);
            Assert.Equal(created.Id, result.Id);
        }
       
        //update test
        [Fact]
        public async Task Should_UpdateLecturer_WhenNewNameIsNew()
        {
            //ülesseade
            LectureDTO dto = new LectureDTO();
            dto.FirstName = "Test";
            dto.LastName = "Test";
            dto.Qualifications = "Test";

            var created = await Svc<ILecturersServices>().Create(dto);

            //tegevus
            LectureDTO update = new LectureDTO();
            update.Id = created.Id;
            update.FirstName = "NewName";
            update.LastName = "Test";
            update.Qualifications = "Test";
            var result = await Svc<ILecturersServices>().Update(update);
            //kontroll
            Assert.NotNull(result);
            Assert.Equal("NewName", result.FirstName);
        }

     
        //delete test
        [Fact]
        public async Task Should_DeleteLecturer_WhenValidIdisGiven()
        {
            //ülesseade
            LectureDTO dto = new LectureDTO();
            dto.FirstName = "Test";
            dto.LastName = "Test";
            dto.Qualifications = "Test";

            var created = await Svc<ILecturersServices>().Create(dto);

            LectureDTO deleteDto = new LectureDTO();
            deleteDto.Id = created.Id;

            //tegevus
            var result = await Svc<ILecturersServices>().Delete(deleteDto);
            var deleted = await Svc<ILecturersServices>().DetailAsync(deleteDto);

            //kontroll
            Assert.Null(deleted);                                        //andmebaasist ei leitud objekti
            Assert.NotNull(result);                                      //delete tagastas kustutatud objekti
            Assert.Equal(created.Id, result.Id);
            Assert.Equal(created.FirstName, result.FirstName);
            Assert.Equal(created.LastName, result.LastName);
            Assert.Equal(created.Qualifications, result.Qualifications);
        }


    }
}