using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace KeelteKoolV2SeleniumTesting
{
    public class LanguageCourseFrontendTests
    {
        private const string BaseUrl = "http://localhost:5234/";

        [Fact]
        public void Should_NavigateToIndex_AddLanguageCourseWithCorrectData_ReturnToIndex()
        {
            // Arrange
            IWebDriver driver = TestSetup_NavigateToIndex();
            try
            {
                // Act
                DataEntry_LanguageCourse(driver);
                driver.FindElement(By.Id("CU_Create_BTN")).Click();

                // otsime just selle rea, kus on meie kursus
                IWebElement row = driver.FindElement(By.XPath("//tr[td[text()='Automaattestimise kursus']]"));
                string nimetus = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
                string keel = row.FindElement(By.Id("IndexTableCourseKeel")).Text;
                string tase = row.FindElement(By.Id("IndexTableCourseTase")).Text;

                // Assert
                Assert.Equal("Automaattestimise kursus", nimetus);
                Assert.Equal("C# + Selenium & xUnit", keel);
                Assert.Equal("3", tase);
            }
            finally
            {
                driver.Quit();
            }
        }

        private static IWebDriver TestSetup_NavigateToIndex()
        {
            IWebDriver driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5); // ootame, kuni element lehele ilmub
            driver.Url = BaseUrl;
            driver.FindElement(By.LinkText("Keelekursused")).Click(); 
            return driver;
        }

        private static void DataEntry_LanguageCourse(IWebDriver driver, bool isOneOrTwo = false)
        {
            if (!isOneOrTwo)
            {
           IWebElement indexCreateButton = driver.FindElement(By.Id("IndexCreateNewLanguageCourse_BTN"));
           indexCreateButton.Click();
            IWebElement createEntryNimetus = driver.FindElement(By.Id("CU_LanguageCourse_Nimetus"));
            createEntryNimetus.SendKeys("Automaattestimise kursus");
            IWebElement createEntryKeel = driver.FindElement(By.Id("CU_LanguageCourse_Keel"));
            createEntryKeel.SendKeys("C# + Selenium & xUnit");
            IWebElement createEntryTase = driver.FindElement(By.Id("CU_LanguageCourse_Tase"));
            createEntryTase.SendKeys("3");
            IWebElement createEntryKirjeldus = driver.FindElement(By.Id("CU_LanguageCourse_Kirjeldus"));
            createEntryKirjeldus.SendKeys("Kursusel õpime, mis on automaattestimine ...");
            }
            else
            {
                IWebElement createEntryNimetus = driver.FindElement(By.Id("CU_LanguageCourse_Nimetus"));
                createEntryNimetus.Clear();
                createEntryNimetus.SendKeys("Manuaaltestimise kursus");
                IWebElement createEntryKeel = driver.FindElement(By.Id("CU_LanguageCourse_Keel"));
                createEntryKeel.Clear();
                createEntryKeel.SendKeys("Mantis, Excel");
                IWebElement createEntryTase = driver.FindElement(By.Id("CU_LanguageCourse_Tase"));
                createEntryTase.Clear();
                createEntryTase.SendKeys("3");
                IWebElement createEntryKirjeldus = driver.FindElement(By.Id("CU_LanguageCourse_Kirjeldus"));
                createEntryKirjeldus.Clear();
                createEntryKirjeldus.SendKeys("Kursusel opime mis on manuaaltestimine, kuidas erineb automaattestimisest ning " +
                "mis dokumentatsioon manuaaltestimise juures tahtis on ning kuidas neid kirjutada.");
            }
        }

        [Fact]
        public void Should_NavigateToDetailsOfLanguageCourse_WithPreviouslyCorrectData()
        {
            // Arrange
            IWebDriver driver = TestSetup_NavigateToIndex();
            var tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0); // eemaldame päise rea
            bool isObjectFound = false;

            /* tegevus */
            foreach (var row in rowsOfTable)
            {
                var nameElement = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
                var keelElement = row.FindElement(By.Id("IndexTableCourseKeel")).Text;
                if ((nameElement == "Automaattestimise kursus") && (keelElement == "C# + Selenium & xUnit"))
                {
                    isObjectFound = true;
                    var clickAction = row
                        .FindElement(By.Id("IndexTableCourseActions"))
                        .FindElement(By.Id("IndexTableCourseDetails_BTN"));
                    clickAction.Click();
                    break;
                }
            }

            if (!isObjectFound)
            {
                driver.Quit();
                Assert.Fail("Kursust 'Automaattestimise kursus' ei leitud Index tabelist.");
            }

            //find data
            string details_id = driver.FindElement(By.Id("DD_LanguageCourseGuid")).Text;
            string details_name = driver.FindElement(By.Id("DD_LanguageCourseNimetus")).Text;
            string details_keel = driver.FindElement(By.Id("DD_LanguageCourseKeel")).Text;
            string details_kirjeldus = driver.FindElement(By.Id("DD_LanguageCourseKirjeldus")).Text;
            string details_tase = driver.FindElement(By.Id("DD_LanguageCourseTase")).Text;
            driver.Quit();

            //assertime
           Assert.NotNull(details_id);

            Assert.True(Guid.TryParse(details_id, out _));
            Assert.Equal("Automaattestimise kursus", details_name);
            Assert.Equal("C# + Selenium & xUnit", details_keel);
            Assert.Equal("Kursusel õpime, mis on automaattestimine ...", details_kirjeldus);
            Assert.Equal("3", details_tase);
        }

        [Fact]
        public void Should_UpdateDetailsOfLanguageCourse_WithNewData()
        {
            /* Ülesseade */
            IWebDriver driver = TestSetup_NavigateToIndex();
            var tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0); // eemaldame päise rea
            bool isObjectFound = false;

            /* tegevus */
            foreach (var row in rowsOfTable)
            {
                var nameElement = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
                var keelElement = row.FindElement(By.Id("IndexTableCourseKeel")).Text;
                if ((nameElement == "Automaattestimise kursus") && (keelElement == "C# + Selenium & xUnit"))
                {
                    isObjectFound = true;
                    var clickAction = row
                        .FindElement(By.Id("IndexTableCourseActions"))
                        .FindElement(By.Id("IndexTableCourseUpdate_BTN"));
                    clickAction.Click(); // avame Update vaate
                    break;
                }
            }

            if (!isObjectFound)
            {
                driver.Quit();
                Assert.Fail("Kursust 'Automaattestimise kursus' ei leitud Index tabelist.");
            }

            DataEntry_LanguageCourse(driver, true); // kirjutame uued andmed
            IWebElement updateButton = driver.FindElement(By.Id("CU_Update_BTN"));
            updateButton.Click(); // salvestame, tagasi Index lehele

            /* kontroll */
            IWebElement updatedRow = driver.FindElement(By.XPath("//tr[td[text()='Manuaaltestimise kursus']]"));
            string updated_name = updatedRow.FindElement(By.Id("IndexTableCourseNimetus")).Text;
            string updated_keel = updatedRow.FindElement(By.Id("IndexTableCourseKeel")).Text;
            string updated_tase = updatedRow.FindElement(By.Id("IndexTableCourseTase")).Text;
            driver.Quit();

            //assertime
            Assert.Equal("Manuaaltestimise kursus", updated_name);
            Assert.Equal("Mantis, Excel", updated_keel);
            Assert.Equal("3", updated_tase);
        }
    }
}