using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace KeeltekoolV2.UnitTesting
{
    public class LanguageCoursesServicesTests : TestBase
    {
        // [Fact] - käsusõna, mida testrunner tunneb, et aru saada mis on test, ja mis ei ole
        // Testi nimi koosneb kolmest osast:
        // 1 - kas test on tavaline (Should) või negatiivne (ShouldNot)
        // 2 - mida parasjagu testiga testitakse
        // 3 - mis tingimustel tulemust kontrollitakse

        [Fact]
        public async Task Should_AddNewCourse_WhenAllFieldsFilled()
        {
            //ülesseade
            LanguageCourseDTO newCourseDTO = MockLanguageCourseDtoData();

            //tegevus
            var result = await Svc<ILanguageCoursesServices>().Create(newCourseDTO);

            //kontroll
            Assert.NotNull(result);                                  //teenus tagastas objekti
            Assert.NotEqual(Guid.Empty, result.Id);                  //objektile anti id
            Assert.Equal(newCourseDTO.Nimetus, result.Nimetus);      //nimetus on sama mis saadeti
            Assert.Equal(newCourseDTO.Keel, result.Keel);            //keel on sama mis saadeti
            Assert.Equal(newCourseDTO.Tase, result.Tase);            //tase on sama mis saadeti
            Assert.Equal(newCourseDTO.Kirjeldus, result.Kirjeldus);  //kirjeldus on sama mis saadeti
        }

        [Fact]
        public async Task ShouldNot_AddNewCourse_WhenFieldsEmpty()
        {
            //ülesseade
            LanguageCourseDTO newCourseDTO = MockLanguageCourseDtoData();
            newCourseDTO.Nimetus = "";
            newCourseDTO.Keel = "";

            //tegevus
            var result = await Svc<ILanguageCoursesServices>().Create(newCourseDTO);

            //kontroll
            Assert.Null(result); //kontrollime et teenus lükkas objekti lisamise tagasi
        }



        [Fact]

        public async Task Should_ReturnCourseDetails_WhenGuidIsNotNull()
        {
            //ülesseade
            //tekitame uue objekti
            LanguageCourseDTO course = MockLanguageCourseDtoData();
            //lisame andmebaasi
            var createdCourse = await Svc<ILanguageCoursesServices>().Create(course);

            //tegevus
            //kasutame objektis asuvat id et see objekt tagasi lugeda andmebaasist DetailsAsync meetodiga
            var result = await Svc<ILanguageCoursesServices>().DetailsAsync(createdCourse.Id);

            //kontroll
            //kontrollime et tagastati midagi
            Assert.NotNull(result);
            //kontrollime et tagastatud objekti id on sama nagu see mis andmebaasi lisatud sai
            Assert.Equal(result.Id, createdCourse.Id);
            Assert.True(result.Id == createdCourse.Id);

            //võrdleme kas objekt on sama nagu see mis genereerisime, va. id-ga
            Assert.Equal(result, createdCourse);
           
        }
        private LanguageCourseDTO MockLanguageCourseDtoData()
        {
            return new LanguageCourseDTO
            {
                Nimetus = "TestKursus",
                Keel = "Eesti keel",
                Tase = "Algtase",
                Kirjeldus = "Ao"
            };
        }
    }
}