using KeeltekoolV2.ApplicationServices.Servises;
using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Text;
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
            dto.FirstName = "Test";
            dto.LastName = "Test";
            dto.Qualifications = "Test";

            var result = await Svc<ILecturersServices>().Create(dto);

            Assert.NotNull(result);
        }

    }
}
