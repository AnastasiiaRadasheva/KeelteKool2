using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace KeelteKoolV2SeleniumTesting
{
    public class LanguageCourseFrontendTests
    {
        [Fact]
        public void Should_NavigateToIndex_AddSpaceshipWithCorrectData_ReturnToIndex()
        {
            // Arrange
            IWebDriver driver = new ChromeDriver();
            driver.Url = "http://localhost:7191/";
            IWebElement navigateToLanguageCourse = driver.FindElement(By.LinkText("Keelekursused"));//otsime tesitava osa lingi   
            navigateToLanguageCourse.Click();//simuleeritud tegevus elemendiga antud juhul klikk
            // Act
            IWebElement indexcreatebutton = driver.FindElement(By.Id("IndexCreateNewLanguageCourse_BTN"));
            indexcreatebutton.Click();

            // Code to create a new course using Selenium WebDriver
            // Assert

            IWebElement indexCreateButton = driver.FindElement(By.Id("IndexCreateNewLanguageCourse_BTN"));
            indexCreateButton.Click();
            IWebElement createEntryNimetus = driver.FindElement(By.Id("CU_LanguageCourse_Nimetus"));
            createEntryNimetus.SendKeys("Automattestimise kursus");

            IWebElement createEntryKeel = driver.FindElement(By.Id("CU_LanguageCourse_Keel"));
            createEntryKeel.SendKeys("c# + selenium xUnit");
            IWebElement createEntryTase = driver.FindElement(By.Id("CU_LanguageCourse_Tase"));
            createEntryTase.SendKeys("3");
            IWebElement createEntryKirjeldus = driver.FindElement(By.Id("CU_LanguageCourse_Kirjeldus"));
            createEntryKirjeldus.SendKeys("Kursus õpime mis on automattetimine kuidas erineb manuaaltestimisest ning kuidas kirjutada backandi ja frontendi");
            IWebElement createcreateKestvus = driver.FindElement(By.Id("CU_Create_BTN"));
            createcreateKestvus.Click();
            // Code to verify that the course was created successfully
        //kogume andmed 
        string LanguageCourseNameData = driver.FindElement(By.Id("IndexTableCourseNimetus")).Text;
        string LanguageCourseKeelData = driver.FindElement(By.Id("IndexTableCourseKeel")).Text;
        string LanguageCourseTaseData = driver.FindElement(By.Id("IndexTableCourseTase")).Text;
            driver.Quit();

            //assertime
            Assert.Equal("Automattestimise kursus", LanguageCourseNameData);
            Assert.Equal("c# + selenium xUnit", LanguageCourseKeelData);
            Assert.Equal("3", LanguageCourseTaseData);
            Assert.True(("Automattestimise kursus"== LanguageCourseNameData) && ("c# + selenium xUnit" == LanguageCourseKeelData) && ("3" == LanguageCourseTaseData));
        }
    }
}
