using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace KeelteKoolV2SeleniumTesting
{
    [TestCaseOrderer("KeelteKoolV2.SeleniumTesting.PriorityOrderer", "KeelteKoolV2.SeleniumTesting")]
    public class LanguageCourseFrontendTests
    {

        public static string thisRunGuid { get; set; }
        //järjesti klass, mis omab omadusi mille järgi teste saab järjestada, väärtusi saab omistada ainult läbi konstruktori argumentide.

        public class OrderAttribute : Attribute
        {
            public int Value { get; } //väärtus
            public OrderAttribute(int value) //omistuskonstruktor
            {
                Value = value;
            }
        }
        // xUnit järjesti mis implementeerib ITestCaseOrdereri liidesest "OrderTestCases" meetodi, 
        // ning see meetod on see, mida me kasutame testide käivitamisel nende testide õigesti järjestamiseks
        public class PriorityOrderer : ITestCaseOrderer
        {
            public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
            {
                // Teostatakse testide sorteerimine ja tagastatakse tulemus kohe return-lausega
                return testCases.OrderBy(test =>
                {
                    // otsitakse järjesti omadusena
                    var method = test.TestMethod.Method.GetCustomAttributes(typeof(OrderAttribute)).FirstOrDefault();

                    // kui ei leita
                    if (method == null)
                    {
                        // antakse tagasi maksimaalne täisarv, ehk test teostatakse kohe, ega ei panda teiste potentsiaalselt ÕIGES järjekorras olevate testide vahele
                        return int.MaxValue;
                    }
                    // kui leitakse
                    else
                    {
                        // siis tagastatakse selle järjesti omaduse väärtus
                        return method.GetNamedArgument<int>(nameof(OrderAttribute.Value));
                    }
                }).ToList(); // testid asetatakse nimekirja
            }
        }



        [Fact]
        [Order(5)]
        public void Should_NavigateToIndex_AddLanguageCourseWithUniqueData_ReturnToIndex()
        {
            /* Ülesseade */
            IWebDriver driver = TestSetup_NavigateToIndex();

            /* tegevus */
            IWebElement indexCreateButton = driver.FindElement(By.Id("IndexCreateNewLanguageCourse_BTN"));
            indexCreateButton.Click(); // //click on create button

            // PARANDATUD RIDA: Lisatud parameeter true
            string uniqueId = DataEntry_LanguageCourse(driver, true); // //enter data in view

            IWebElement CreateCreateButton = driver.FindElement(By.Id("CU_Create_BTN"));
            CreateCreateButton.Click(); // //click create button in create view

            /* kontroll */

            //kogume andmed
            string languageCourseNameData = driver.FindElement(By.Id("IndexTableCourseNimetus")).Text;
            string languageCourseKeelData = driver.FindElement(By.Id("IndexTableCourseKeel")).Text;
            //sulgeme akna
            Thread.Sleep(500);
            driver.Quit();
            //assertine
            Assert.Equal(uniqueId, languageCourseNameData);
            Assert.Equal("C# + Selenium & xUnit", languageCourseKeelData);
            Assert.True((uniqueId == languageCourseNameData) && ("C# + Selenium & xUnit" == languageCourseKeelData));
        }

        /// <summary>
        /// Ülesseade meetod mis tekitab navigeerib aadresile ja vajutab Keelltekursustele lingile
        /// </summary>
        /// <returns>tagastab testisiseselt kasuatatava draiveri</returns>
        private static IWebDriver TestSetup_NavigateToIndex()
        {
            IWebDriver driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5); // ootame, kuni element lehele ilmub
            driver.Url = BaseUrl;
            driver.FindElement(By.LinkText("Keelekursused")).Click(); 
            return driver;
        }


        /// <summary>
        /// Meetod mis testimise eesmärgi siseldab lehel andmeid on võimalik konfigureerida
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="isOneOrTwo"> Muutuja mis valib kare erineva andmekomplekti vahel</param>
        /// <param name="updateName"> Muutuja mis ütleb kas nime on vaja muuta nimes hoitaksme testimiseks vajalikku unikaalset Guidi</param>
        /// <returns>Tagastab lisatud Guidi stringi kujul miś pangi nimevälja kui on konfiguureritud nii</returns>
        private static string DataEntry_LanguageCourse(IWebDriver driver, bool isOneOrTwo= false, bool updateName = true)
        {

            Guid generatedId = Guid.NewGuid();
            if (updateName)
            {
                IWebElement createEntryNimetus = driver.FindElement(By.Id("CU_LanguageCourse_Nimetus"));
                createEntryNimetus.SendKeys(generatedId.ToString());
            }
      

            if (!isOneOrTwo)
    {
                IWebElement createEntryKeel = driver.FindElement(By.Id("CU_LanguageCourse_Keel"));
                createEntryKeel.SendKeys("C# + Selenium & xUnit");
                IWebElement createEntryTase = driver.FindElement(By.Id("CU_LanguageCourse_Tase"));
                createEntryTase.SendKeys("3");
                IWebElement createEntryKirjeldus = driver.FindElement(By.Id("CU_LanguageCourse_Kirjeldus"));
                createEntryKirjeldus.SendKeys("Kursusel õpime mis on automaattestimine, kuidas erineb manuaaltestimisest ning kuidas kirjutada " +
                    "backendi ja frontendi automaatteste.");
            }
            else
            {
                IWebElement createEntryKeel = driver.FindElement(By.Id("CU_LanguageCourse_Keel"));
                createEntryKeel.SendKeys("Mantis, Excel");
                IWebElement createEntryTase = driver.FindElement(By.Id("CU_LanguageCourse_Tase"));
                createEntryTase.SendKeys("4");
                IWebElement createEntryKirjeldus = driver.FindElement(By.Id("CU_LanguageCourse_Kirjeldus"));
                createEntryKirjeldus.SendKeys("Kursusel õpime mis on manuaaltestimine, kuidas erineb automaattestimisest ning " +
                    "mis dokumentatsioon manuaaltestimise juures tähtis on ning kuidas neid kirjutada.");
            }

            return generatedId.ToString();
        }



        /// <summary>
        /// Test which updates on object with new data
        /// </summary>
        [Fact]
        [Order(6)]
        public void Should_NavigateToDetailsOfLanguageCourse_WithPreviouslyUniqueData()
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
                if ((nameElement == "Automaattestimise kursus") )
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
                //}
                //DataClear_CreateUpdate(driver);
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
                Assert.Equal("Kursusel õpime mis on manuaaltestimine, kuidas erineb automaattestimisest ning " +
                    "mis dokumentatsioon manuaaltestimise juures tähtis on ning kuidas neid kirjutada.", details_kirjeldus);
                Assert.True(int.Parse(details_tase)> -1);

            }
        }


        /// <summary>
        /// puhustab andmeväkad meetodile kaasastus draiveri sessioonis
        /// </summary>
        /// <param name="driver">Hetkel käsitletav draiver</param>
        private void DataClear_CreateUpdate(IWebDriver driver)
        {
          

            IWebElement createEntryNimetus = driver.FindElement(By.Id("CU_LanguageCourse_Nimetus"));
            createEntryNimetus.Clear();
            IWebElement createEntryKeel = driver.FindElement(By.Id("CU_LanguageCourse_Keel"));
            createEntryKeel.Clear();
            IWebElement createEntryTase = driver.FindElement(By.Id("CU_LanguageCourse_Tase"));
            createEntryTase.Clear();
            IWebElement createEntryKirjeldus = driver.FindElement(By.Id("CU_LanguageCourse_Kirjeldus"));
            createEntryKirjeldus.Clear();

        }

        private const string BaseUrl = "http://localhost:5234/";

        /// <summary>
        /// The whish updates a specific previously added object with new dataaaaa
        /// </summary>
        [Fact]
        [Order(7)]
        public void Should_UpdateDetailsOfUniquleLanguageCourse_WithNewData()
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

            DataEntry_LanguageCourse_SaveName(driver); // kirjutame uued andmed

            DataEntry_LanguageCourse(driver, true, false); // kirjutame uued andmed
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
            Assert.NotEmpty(updated_name);
            Assert.NotEmpty(updated_keel);


        }


        /// <summary>
        /// test which removes a specific previosly added object
        /// </summary>
        [Fact]
        [Order(8)]
        public void Should_DeleteLanguageCourse_FromDeleteUmiqueView()
        {
            /* ülesseade */
            IWebDriver driver = TestSetup_NavigateToIndex();
            var tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            bool isObjectFound = false;

            /* tegevus */
            foreach (var row in rowsOfTable)
            {
                var nameElement = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
            
                if ((nameElement == "Manuaaltestimise kursus") )
                {
                    isObjectFound = true;
                    var clickAction = row
                        .FindElement(By.Id("IndexTableCourseActions"))
                        .FindElement(By.Id("IndexTableCourseDetails_BTN"));
                    clickAction.Click();
                    break;
                }
            }
            Thread.Sleep(1000);
            IWebElement clickDelete = driver.FindElement(By.Id("DeleteDelete_BTN"));
            clickDelete.Click();

            //kontroll
            isObjectFound = false;
            tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            isObjectFound = false;

            /* tegevus */
            foreach (var row in rowsOfTable)
            {
                var nameElement = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
                if ((nameElement == "Manuaaltestimise kursus"))
                {
                    isObjectFound = true;
                    var clickAction = row
                        .FindElement(By.Id("IndexTableCourseActions"))
                        .FindElement(By.Id("IndexTableCourseDetails_BTN"));
                    clickAction.Click();
                    break;
                }
            }
            Assert.False(isObjectFound);
            Assert.True(!isObjectFound);
        }




        /// <summary>
        /// puhustab andmeväkad meetodile kaasastus draiveri sessioonis
        /// välja arvatud mi,e väli
        /// </summary>
        /// <param name="driver">Hetkel käsitletav draiver</param>
        private void DataEntry_LanguageCourse_SaveName(IWebDriver driver)
        {
 
            IWebElement createEntryKeel = driver.FindElement(By.Id("CU_LanguageCourse_Keel"));
            createEntryKeel.Clear();
            IWebElement createEntryTase = driver.FindElement(By.Id("CU_LanguageCourse_Tase"));
            createEntryTase.Clear();
            IWebElement createEntryKirjeldus = driver.FindElement(By.Id("CU_LanguageCourse_Kirjeldus"));
            createEntryKirjeldus.Clear();
        }
    }

}