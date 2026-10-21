using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
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

        //abimeetodid

        private async Task<Lecturer> AddObjectToDB()
        {
            LectureDTO dto = MockLecturerDtoData();
            var created = await Svc<ILecturersServices>().Create(dto);
            return created;
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
