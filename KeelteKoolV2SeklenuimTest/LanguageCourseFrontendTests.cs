using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace KeelteKoolV2SeleniumTesting
{
    //järjesti klass, mis omab omadusi mille järgi teste saab järjestada, väärtusi saab omistada ainult läbi konstruktori argumentide.
    public class OrderAttribute : Attribute
    {
        public int Value { get; } //väärtus
        public OrderAttribute(int value) //omistuskonstruktor
        {
            Value = value;
        }
    }

    //xUnit järjesti mis implementeerib ITestCaseOrdereri liidesest "OrderTestCases" meetodi, ning see meetod on see, mida me kasutame
    //testide käivitamisel nende testide õigesti järjestamiseks. Järjestatakse vastavalt OrderAttribute omadusele
    public class PriorityOrderer : ITestCaseOrderer
    {
        //meetod võtab sisse kõik test cased
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            //toimub sorteerimistegevus kus järjestatakse arendaja kirjutatud järjesti järgi.
            var result = testCases.OrderBy(test =>
            {
                //otsitakse järjesti omadusena
                var method = test.TestMethod.Method.GetCustomAttributes(typeof(OrderAttribute)).FirstOrDefault();
                //kui ei leita
                if (method == null)
                {
                    //antakse tagasi maksimaalne täisarv, ehk test teostatakse viimasena
                    return int.MaxValue;
                }
                //kui leitakse
                else
                {
                    //siis tagastatakse selle järjesti omaduse väärtus
                    return method.GetNamedArgument<int>(nameof(OrderAttribute.Value));
                }
            }).ToList(); //testid asetatakse nimekirja

            return result; //sorteeritud tulemus tagastatakse
        }
    }

    [TestCaseOrderer("KeelteKoolV2SeleniumTesting.PriorityOrderer", "KeelteKoolV2SeleniumTesting")]
    public class LanguageCourseFrontendTests
    {
        public static string thisRunGuid { get; set; } = "";

        /// <summary>
        /// Test which creates a new object, holding a unique id in the "Name" field
        /// </summary>
        [Fact]
        [Order(5)]
        public void Should_NavigateToIndex_AddLanguageCourseWithUniqueData_ReturnToIndex()
        {
            /* Ülesseade */
            IWebDriver driver = TestSetup_NavigateToIndex();

            /* tegevus */
            IWebElement indexCreateButton = driver.FindElement(By.Id("IndexCreateNewLanguageCourse_BTN"));
            indexCreateButton.Click(); //click on create button

            string uniqueId = DataEntry_LanguageCourse(driver); //enter data in view
            thisRunGuid = uniqueId;

            IWebElement CreateCreateButton = driver.FindElement(By.Id("CU_Create_BTN"));
            CreateCreateButton.Click(); //click create button in create view

            /* kontroll */

            //kogume andmed
            var tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            string LanguageCourseNameData = "";
            string LanguageCourseKeelData = "";
            foreach (var row in rowsOfTable)
            {
                if (row.FindElement(By.Id("IndexTableCourseNimetus")).Text == thisRunGuid)
                {
                    LanguageCourseNameData = thisRunGuid;
                    LanguageCourseKeelData = row.FindElement(By.Id("IndexTableCourseKeel")).Text;
                }
            }
            //sulgeme akna
            Thread.Sleep(500);
            driver.Quit();
            //assertime
            Assert.Equal(uniqueId, LanguageCourseNameData);
            Assert.Equal("C# + Selenium & xUnit", LanguageCourseKeelData);
            Assert.True((uniqueId == LanguageCourseNameData) && ("C# + Selenium & xUnit" == LanguageCourseKeelData));
        }

        /// <summary>
        /// Test which navigates to the details of a previously added specific object
        /// </summary>
        [Fact]
        [Order(6)]
        public void Should_NavigateToDetailsOfALanguageCourse_WithPreviouslyUniqueData()
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
                if (nameElement == thisRunGuid)
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
            //find data
            string details_id = driver.FindElement(By.Id("DD_LanguageCourseGuid")).Text;
            string details_name = driver.FindElement(By.Id("DD_LanguageCourseNimetus")).Text; //omab testimise jaoks vajalikku id-d
            string details_keel = driver.FindElement(By.Id("DD_LanguageCourseKeel")).Text;
            string details_kirjeldus = driver.FindElement(By.Id("DD_LanguageCourseKirjeldus")).Text;
            string details_tase = driver.FindElement(By.Id("DD_LanguageCourseTase")).Text;
            IWebElement goBack = driver.FindElement(By.Id("DetailsBack_BTN"));
            goBack.Click();
            Thread.Sleep(1000);
            driver.Quit();

            /* Kontroll */
            Assert.True(isObjectFound);
            Assert.NotEmpty(details_id);
            Assert.NotEmpty(details_name);
            Assert.NotEmpty(details_keel);
            Assert.NotEmpty(details_kirjeldus);
            Assert.NotEmpty(details_tase);
            Assert.Equal(thisRunGuid, details_name);
            Assert.Equal("C# + Selenium & xUnit", details_keel);
            Assert.Equal("Kursusel õpime mis on automaattestimine, kuidas erineb manuaaltestimisest ning kuidas kirjutada " +
                    "backendi ja frontendi automaatteste.", details_kirjeldus);
            Assert.Equal("3", details_tase);
            Assert.True(int.Parse(details_tase) > -1);
        }

        /// <summary>
        /// Test which updates a specific previously added object with new data
        /// </summary>
        [Fact]
        [Order(7)]
        public void Should_UpdateDetailsOfUniqueLanguageCourse_WithNewData()
        {
            /* Ülesseade */
            IWebDriver driver = TestSetup_NavigateToIndex();
            var tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            bool isObjectFound = false;

            /* tegevus */
            foreach (var row in rowsOfTable)
            {
                var nameElement = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
                if (nameElement == thisRunGuid)
                {
                    isObjectFound = true;
                    var clickAction = row
                        .FindElement(By.Id("IndexTableCourseActions"))
                        .FindElement(By.Id("IndexTableCourseUpdate_BTN"));
                    clickAction.Click();
                    break;
                }
            }
            Thread.Sleep(500);
            DataClear_CreateUpdate_SaveName(driver);
            DataEntry_LanguageCourse(driver, true, false);
            IWebElement updateclick = driver.FindElement(By.Id("CU_Update_BTN"));
            updateclick.Click();
            IWebElement goBack = driver.FindElement(By.Id("CU_Cancel_BTN"));
            goBack.Click();

            /* kontrolle */
            //otsi objekt üles:
            tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            isObjectFound = false;
            Thread.Sleep(1000);
            foreach (var row in rowsOfTable)
            {
                var nameElement = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
                if (nameElement == thisRunGuid)
                {
                    isObjectFound = true;
                    var clickAction = row
                        .FindElement(By.Id("IndexTableCourseActions"))
                        .FindElement(By.Id("IndexTableCourseDetails_BTN"));
                    clickAction.Click();
                    break;
                }
            }
            //find data
            string details_id = driver.FindElement(By.Id("DD_LanguageCourseGuid")).Text;
            string details_name = driver.FindElement(By.Id("DD_LanguageCourseNimetus")).Text; //unikaalne id
            string details_keel = driver.FindElement(By.Id("DD_LanguageCourseKeel")).Text;
            string details_kirjeldus = driver.FindElement(By.Id("DD_LanguageCourseKirjeldus")).Text;
            string details_tase = driver.FindElement(By.Id("DD_LanguageCourseTase")).Text;
            Thread.Sleep(1000);
            driver.Quit();

            Assert.True(isObjectFound);
            Assert.NotNull(details_id);
            Assert.NotEmpty(details_id);
            Assert.NotEmpty(details_name);
            Assert.Equal(thisRunGuid, details_name);
            Assert.Equal("Mantis, Excel", details_keel);
            Assert.Equal("4", details_tase);
            Assert.NotEqual("Kursusel õpime mis on automaattestimine, kuidas erineb manuaaltestimisest ning kuidas kirjutada " +
                    "backendi ja frontendi automaatteste.", details_kirjeldus);
        }

        /// <summary>
        /// Test which removes a specific previously added object
        /// </summary>
        [Fact]
        [Order(8)]
        public void Should_DeleteUniqueLanguageCourse_FromDeleteView()
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
                if (nameElement == thisRunGuid)
                {
                    isObjectFound = true;
                    var clickAction = row
                        .FindElement(By.Id("IndexTableCourseActions"))
                        .FindElement(By.Id("IndexTableCourseDelete_BTN"));
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
            foreach (var row in rowsOfTable)
            {
                var nameElement = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
                if (nameElement == thisRunGuid)
                {
                    isObjectFound = true;
                    break;
                }
            }
            driver.Quit();

            Assert.False(isObjectFound);
            Assert.True(!isObjectFound);
        }

        //eof

        /// <summary>
        /// Puhastab andmeväljad meetodile kaasaantud draiveri sessioonis
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

        /// <summary>
        /// Puhastab andmeväljad meetodile kaasaantud draiveri sessioonis
        /// välja arvatud nime väli
        /// </summary>
        /// <param name="driver">Hetkel käsitletav draiver</param>
        private void DataClear_CreateUpdate_SaveName(IWebDriver driver)
        {
            IWebElement createEntryKeel = driver.FindElement(By.Id("CU_LanguageCourse_Keel"));
            createEntryKeel.Clear();
            IWebElement createEntryTase = driver.FindElement(By.Id("CU_LanguageCourse_Tase"));
            createEntryTase.Clear();
            IWebElement createEntryKirjeldus = driver.FindElement(By.Id("CU_LanguageCourse_Kirjeldus"));
            createEntryKirjeldus.Clear();
        }

        /// <summary>
        /// Meetod mis testimise eesmärgil sisestab lehel andmeid, on võimalik konfigureerida.
        /// </summary>
        /// <param name="driver">WebDriver mida hetkel test rakendab</param>
        /// <param name="isOneOrTwo">Muutuja mis valib kahe erineva andmekomplekti vahel</param>
        /// <param name="updateName">Muutuja mis ütleb kas nime on vaja muuta (nimes hoitakse testimiseks vajalikku unikaalset Guidi)</param>
        /// <returns>Tagastab lisatud Guidi stringi kujul mis pandi nimevälja (Kui on konfigureeritud nii)</returns>
        private static string DataEntry_LanguageCourse(IWebDriver driver, bool isOneOrTwo = false, bool updateName = true)
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
        /// Ülesseade meetod mis tekitab draiveri, navigeerib aadressile ja vajutab Keeltekursuste lingile.
        /// </summary>
        /// <returns>Tagastab testisiseselt kasutatava draiveri</returns>
        private static IWebDriver TestSetup_NavigateToIndex()
        {
            IWebDriver driver = new ChromeDriver(); //brauserit juhtiv ja seal tegutsev draiver
            driver.Manage().Window.Maximize(); //suur aken, et footer ei kataks nuppe
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5); //ootame, kuni element lehele ilmub
            driver.Url = "http://localhost:5234/"; //navigeeritav aadress
            IWebElement navigateToLanguageCourse = driver.FindElement(By.LinkText("Keelekursused"));//otsime tesitava osa lingi
            navigateToLanguageCourse.Click(); //simuleeritud tegevus elemendiga, antud juhul klikk
            return driver;
        }
    }
}
