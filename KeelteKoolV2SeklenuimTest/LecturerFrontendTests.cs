using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace KeelteKoolV2SeleniumTesting
{
    //Õppejõudude (Lecturers) kasutajaliidese testid. Testid käivitatakse kindlas järjekorras (Order),
    //sest Create test loob õppejõu, keda järgmised testid vaatavad, muudavad ja viimane test kustutab.
    [TestCaseOrderer("KeelteKoolV2SeleniumTesting.PriorityOrderer", "KeelteKoolV2SeleniumTesting")]
    public class LecturerFrontendTests
    {
        public static string thisRunGuid { get; set; } = "";

        /// <summary>
        /// CREATE test: avab õppejõudude nimekirja, vajutab "Lisa uus õppejõud", sisestab eesnimeks unikaalse
        /// Guidi ning perenime ja kvalifikatsioonid ja salvestab. Kontrollib, et suunati tagasi nimekirja,
        /// tabelis on täpselt üks uus rida ja selles reas on just need andmed, mis vormi sisestati.
        /// </summary>
        [Fact]
        [Order(2)]
        public void Should_NavigateToIndex_AddLecturerWithUniqueData_ReturnToIndex()
        {
            /* Ülesseade */
            IWebDriver driver = TestSetup_NavigateToIndex();
            var tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            int rowsBefore = rowsOfTable.Count;

            /* tegevus */
            IWebElement indexCreateButton = driver.FindElement(By.Id("IndexCreateNewLecturer_BTN"));
            indexCreateButton.Click(); //vajutame lisamise nupule
            WaitForNewPage(indexCreateButton); //ootame, kuni uus leht laeb
            string createHeading = driver.FindElement(By.TagName("h1")).Text;

            string uniqueId = DataEntry_Lecturer(driver); //sisestame andmed vormi
            thisRunGuid = uniqueId;

            IWebElement CreateCreateButton = driver.FindElement(By.Id("CU_Create_BTN"));
            CreateCreateButton.Click(); //vajutame vormis salvestamise nupule
            WaitForNewPage(CreateCreateButton); //ootame, kuni uus leht laeb
            Thread.Sleep(1000); //ootame, kuni Index leht laeb

            /* kontroll */

            //kogume andmed
            tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            int rowsAfter = rowsOfTable.Count;
            int foundCount = 0;
            string LecturerLastNameData = "";
            string LecturerQualificationsData = "";
            foreach (var row in rowsOfTable)
            {
                if (row.FindElement(By.Id("IndexTableLecturerFirstName")).Text == thisRunGuid)
                {
                    foundCount++;
                    LecturerLastNameData = row.FindElement(By.Id("IndexTableLecturerLastName")).Text;
                    LecturerQualificationsData = row.FindElement(By.Id("IndexTableLecturerQualifications")).Text;
                }
            }
            string urlAfterSave = driver.Url;
            //sulgeme akna
            Thread.Sleep(500);
            driver.Quit();
            //assertime
            Assert.Equal("Lisa uus õppejõud", createHeading);              //avati lisamise vorm
            Assert.EndsWith("/Lecturers", urlAfterSave);                   //salvestamise järel suunati nimekirja
            Assert.Equal(1, foundCount);                                   //uus õppejõud on tabelis täpselt üks kord
            Assert.Equal(rowsBefore + 1, rowsAfter);                       //tabelisse lisandus üks rida
            Assert.Equal("Oleinik", LecturerLastNameData);
            Assert.Equal("Inglise keel C1, IELTS", LecturerQualificationsData);
            Assert.True(Guid.TryParse(thisRunGuid, out _));                //eesnimes on korrektne Guid
        }

        //eof

        /// <summary>
        /// Puhastab andmeväljad meetodile kaasaantud draiveri sessioonis
        /// </summary>
        /// <param name="driver">Hetkel käsitletav draiver</param>
        private void DataClear_CreateUpdate(IWebDriver driver)
        {
            IWebElement createEntryFirstName = driver.FindElement(By.Id("CU_Lecturer_FirstName"));
            createEntryFirstName.Clear();
            IWebElement createEntryLastName = driver.FindElement(By.Id("CU_Lecturer_LastName"));
            createEntryLastName.Clear();
            IWebElement createEntryQualifications = driver.FindElement(By.Id("CU_Lecturer_Qualifications"));
            createEntryQualifications.Clear();
        }

        /// <summary>
        /// Puhastab andmeväljad meetodile kaasaantud draiveri sessioonis
        /// välja arvatud eesnime väli (eesnimes hoitakse testimiseks vajalikku unikaalset Guidi)
        /// </summary>
        /// <param name="driver">Hetkel käsitletav draiver</param>
        private void DataClear_CreateUpdate_SaveName(IWebDriver driver)
        {
            IWebElement createEntryLastName = driver.FindElement(By.Id("CU_Lecturer_LastName"));
            createEntryLastName.Clear();
            IWebElement createEntryQualifications = driver.FindElement(By.Id("CU_Lecturer_Qualifications"));
            createEntryQualifications.Clear();
        }

        /// <summary>
        /// Meetod mis testimise eesmärgil sisestab lehel õppejõu andmeid, on võimalik konfigureerida.
        /// </summary>
        /// <param name="driver">WebDriver mida hetkel test rakendab</param>
        /// <param name="isOneOrTwo">Muutuja mis valib kahe erineva andmekomplekti vahel</param>
        /// <param name="updateName">Muutuja mis ütleb kas eesnime on vaja muuta (eesnimes hoitakse testimiseks vajalikku unikaalset Guidi)</param>
        /// <returns>Tagastab lisatud Guidi stringi kujul mis pandi eesnime välja (Kui on konfigureeritud nii)</returns>
        private static string DataEntry_Lecturer(IWebDriver driver, bool isOneOrTwo = false, bool updateName = true)
        {
            Guid generatedId = Guid.NewGuid();
            if (updateName)
            {
                IWebElement createEntryFirstName = driver.FindElement(By.Id("CU_Lecturer_FirstName"));
                createEntryFirstName.SendKeys(generatedId.ToString());
            }

            if (!isOneOrTwo)
            {
                IWebElement createEntryLastName = driver.FindElement(By.Id("CU_Lecturer_LastName"));
                createEntryLastName.SendKeys("Oleinik");
                IWebElement createEntryQualifications = driver.FindElement(By.Id("CU_Lecturer_Qualifications"));
                createEntryQualifications.SendKeys("Inglise keel C1, IELTS");
            }
            else
            {
                IWebElement createEntryLastName = driver.FindElement(By.Id("CU_Lecturer_LastName"));
                createEntryLastName.SendKeys("Speek");
                IWebElement createEntryQualifications = driver.FindElement(By.Id("CU_Lecturer_Qualifications"));
                createEntryQualifications.SendKeys("Saksa keel B2, eksamineerija");
            }
            return generatedId.ToString();
        }

        /// <summary>
        /// Ülesseade meetod mis tekitab draiveri, navigeerib aadressile ja vajutab Õppejõudude lingile.
        /// </summary>
        /// <returns>Tagastab testisiseselt kasutatava draiveri</returns>
        private static IWebDriver TestSetup_NavigateToIndex()
        {
            IWebDriver driver = new ChromeDriver(); //brauserit juhtiv ja seal tegutsev draiver
            driver.Manage().Window.Maximize(); //suur aken, et footer ei kataks nuppe
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5); //ootame, kuni element lehele ilmub
            driver.Url = "http://localhost:5234/"; //navigeeritav aadress
            IWebElement navigateToLecturers = driver.FindElement(By.LinkText("Õppejõud"));//otsime testitava osa lingi
            navigateToLecturers.Click(); //simuleeritud tegevus elemendiga, antud juhul klikk
            WaitForNewPage(navigateToLecturers); //ootame, kuni uus leht laeb
            return driver;
        }
        /// <summary>
        /// Ootab, kuni pärast klikki vana leht kaob (element muutub "stale"), ehk uus leht on laadimas/laetud.
        /// </summary>
        /// <param name="clickedElement">Element millele just klikiti</param>
        private static void WaitForNewPage(IWebElement clickedElement)
        {
            for (int i = 0; i < 100; i++) //max 10 sekundit
            {
                try
                {
                    _ = clickedElement.Enabled; //kui vana leht on veel alles, see õnnestub
                    Thread.Sleep(100);
                }
                catch (StaleElementReferenceException)
                {
                    return; //vana leht on kadunud
                }
            }
        }
    }
}
