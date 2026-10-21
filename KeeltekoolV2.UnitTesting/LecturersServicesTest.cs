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

        //abimeetodid

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
