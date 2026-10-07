using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;
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
            ////kontrollime et tagastatud objekti id on sama nagu see mis andmebaasi lisatud sai
            Assert.Equal(result.Id, createdCourse.Id);
            Assert.True(result.Id == createdCourse.Id);

            ////võrdleme kas objekt on sama nagu see mis genereerisime, va. id-ga
            Assert.Equal(result, createdCourse);
           
        }

        [Fact]
      
        public async Task Should_UpdateWithNewData_WhenDataIsDifferentFromDB()
        {
            //ülesseade
            //lisame andmebaasi kursuse
            Core.Domain.LanguageCourse createdCourse = await AddObjectToDB();

            LanguageCourseDTO updatedCourse = new LanguageCourseDTO();
            updatedCourse.Id = createdCourse.Id;
            updatedCourse.Keel = createdCourse.Keel;
            updatedCourse.Tase = createdCourse.Tase;
            updatedCourse.Kirjeldus = createdCourse.Kirjeldus;
            updatedCourse.Nimetus = "Uusnimetus";
            updatedCourse.ModifiedAt = DateTime.UtcNow;

            //tegevus
            await Svc<ILanguageCoursesServices>().Update(updatedCourse);

            //kontroll
            var result = await Svc<ILanguageCoursesServices>().DetailsAsync(createdCourse.Id);

            Assert.NotNull(result);
            Assert.Equal(updatedCourse.Id, result.Id);
            Assert.Equal(updatedCourse.Nimetus, result.Nimetus);
            Assert.Equal(updatedCourse.Keel, result.Keel);
            Assert.Equal(updatedCourse.Tase, result.Tase);
            Assert.Equal(updatedCourse.Kirjeldus, result.Kirjeldus);
        }

        private async Task<LanguageCourse> AddObjectToDB()
        {
            LanguageCourseDTO dto = MockLanguageCourseDtoData();
            var created = await Svc<ILanguageCoursesServices>().Create(dto);
            return created;
        }
      
      public async Task Should_DeleteDataFromDB_WhenIdIsValid()
        {
            //ülesseade
            //lisame andmebaasi kursuse
            Core.Domain.LanguageCourse createdCourse = await AddObjectToDB();
            //tegevus
            var result = await Svc<ILanguageCoursesServices>().Delete(createdCourse.Id);
            var deleted = await Svc<ILanguageCoursesServices>().DetailsAsync(createdCourse.Id);

            //kontroll

            Assert.Null(deleted);
            Assert.NotNull(result);
            //kontrollime et andmebaasist ei leitud objekti

            Assert.Equal(createdCourse.Id, result.Id);
            Assert.Equal(createdCourse.Nimetus, result.Nimetus);
            Assert.Equal(createdCourse.Keel, result.Keel);
            Assert.Equal(createdCourse.Tase, result.Tase);
            Assert.Equal(createdCourse.Kirjeldus, result.Kirjeldus);

            Assert.NotEqual(createdCourse, result);
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