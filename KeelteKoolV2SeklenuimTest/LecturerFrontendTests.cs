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
        /// Oma (negatiivne) test: kui lisamise vormis jäetakse eesnimi ja perenimi tühjaks, ei tohi õppejõudu lisada.
        /// Kontrollib, et jäädakse lisamise vormile, mõlema välja juures on veateade, kvalifikatsioonide
        /// väljale kirjutatud tekst ei kao ära ning õppejõudude tabelisse ei tekkinud uut rida.
        /// </summary>
        [Fact]
        [Order(1)]
        public void ShouldNot_AddLecturer_WhenNamesAreEmpty()
        {
            /* Ülesseade */
            IWebDriver driver = TestSetup_NavigateToIndex();
            var tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            int rowsBefore = rowsOfTable.Count; //mitu õppejõudu on enne proovimist

            /* tegevus */
            IWebElement indexCreateButton = driver.FindElement(By.Id("IndexCreateNewLecturer_BTN"));
            indexCreateButton.Click(); //vajutame lisamise nupule
            WaitForNewPage(indexCreateButton); //ootame, kuni uus leht laeb
            //eesnimi ja perenimi jäävad tühjaks, täidame ainult kvalifikatsioonid
            IWebElement createEntryQualifications = driver.FindElement(By.Id("CU_Lecturer_Qualifications"));
            createEntryQualifications.SendKeys("Ainult kvalifikatsioon");
            IWebElement CreateCreateButton = driver.FindElement(By.Id("CU_Create_BTN"));
            CreateCreateButton.Click(); //vajutame vormis salvestamise nupule
            WaitForNewPage(CreateCreateButton); //ootame, kuni vorm uuesti laeb
            Thread.Sleep(500);

            //kogume vormi andmed pärast salvestamise proovimist
            string urlAfterSave = driver.Url;
            string firstNameError = driver.FindElement(By.CssSelector("span[data-valmsg-for='FirstName']")).Text;
            string lastNameError = driver.FindElement(By.CssSelector("span[data-valmsg-for='LastName']")).Text;

            //läheme "Tagasi" nupuga nimekirja ja loeme read uuesti üle
            IWebElement goBack = driver.FindElement(By.Id("CU_Cancel_BTN"));
            goBack.Click();
            WaitForNewPage(goBack); //ootame, kuni uus leht laeb
            Thread.Sleep(1000); //ootame, kuni Index leht laeb

            /* kontroll */
            tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            int rowsAfter = rowsOfTable.Count;
            bool isEmptyLecturerFound = false;
            foreach (var row in rowsOfTable)
            {
                if (row.FindElement(By.Id("IndexTableLecturerQualifications")).Text == "Ainult kvalifikatsioon")
                {
                    isEmptyLecturerFound = true;
                }
            }
            driver.Quit();

            Assert.Contains("/Lecturers/Create", urlAfterSave);            //jäädi lisamise vormile
            Assert.Equal("Eesnimi on kohustuslik", firstNameError);        //eesnime veateade on näha
            Assert.Equal("Perenimi on kohustuslik", lastNameError);        //perenime veateade on näha
            Assert.Equal(rowsBefore, rowsAfter);                           //õppejõudude arv ei muutunud
            Assert.False(isEmptyLecturerFound);                            //nimedeta õppejõudu tabelis ei ole
        }

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

        /// <summary>
        /// READ test: leiab eelmises testis lisatud õppejõu nimekirjast, avab nupuga "Andmed" tema detailvaate
        /// ja kontrollib, et avati just selle õppejõu leht ja seal on õiged andmed. Detailvaates peavad olema
        /// nupud "Muuda" ja "Tagasi", kuid mitte kustutamise nupp. Lõpuks viib "Tagasi" nimekirja.
        /// </summary>
        [Fact]
        [Order(3)]
        public void Should_NavigateToDetailsOfALecturer_WithPreviouslyUniqueData()
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
                var nameElement = row.FindElement(By.Id("IndexTableLecturerFirstName")).Text;
                if (nameElement == thisRunGuid)
                {
                    isObjectFound = true;
                    var clickAction = row
                        .FindElement(By.Id("IndexTableLecturerActions"))
                        .FindElement(By.Id("IndexTableLecturerDetails_BTN"));
                    clickAction.Click();
                    WaitForNewPage(clickAction); //ootame, kuni uus leht laeb
                    break;
                }
            }
            Thread.Sleep(1000);
            //kogume detailvaate andmed
            string details_heading = driver.FindElement(By.TagName("h1")).Text;
            string details_id = driver.FindElement(By.Id("DD_LecturerGuid")).Text;
            string details_firstName = driver.FindElement(By.Id("DD_LecturerFirstName")).Text; //omab testimise jaoks vajalikku id-d
            string details_lastName = driver.FindElement(By.Id("DD_LecturerLastName")).Text;
            string details_qualifications = driver.FindElement(By.Id("DD_LecturerQualifications")).Text;
            string detailsUrl = driver.Url;
            int updateButtons = driver.FindElements(By.Id("DetailsUpdate_BTN")).Count;
            int deleteButtons = driver.FindElements(By.Id("DeleteDelete_BTN")).Count;
            IWebElement goBack = driver.FindElement(By.Id("DetailsBack_BTN"));
            goBack.Click();
            WaitForNewPage(goBack); //ootame, kuni uus leht laeb
            Thread.Sleep(1000);
            string urlAfterBack = driver.Url;
            driver.Quit();

            /* Kontroll */
            Assert.True(isObjectFound);                                    //õppejõud leiti nimekirjast
            Assert.Equal("Info", details_heading);                         //avati detailvaade, mitte kustutamise vaade
            Assert.NotEmpty(details_id);
            Assert.True(Guid.TryParse(details_id, out _));                 //detailvaates on korrektne id
            Assert.Contains("/Lecturers/Details/" + details_id, detailsUrl); //avati just selle õppejõu leht
            Assert.Equal(thisRunGuid, details_firstName);
            Assert.Equal("Oleinik", details_lastName);
            Assert.Equal("Inglise keel C1, IELTS", details_qualifications);
            Assert.Equal(1, updateButtons);                                //detailvaates on "Muuda" nupp
            Assert.Equal(0, deleteButtons);                                //detailvaates ei ole kustutamise nuppu
            Assert.EndsWith("/Lecturers", urlAfterBack);                   //"Tagasi" viis nimekirja
        }

        /// <summary>
        /// UPDATE test: avab varem lisatud õppejõu muutmise vormi, kontrollib et vormis on vanad andmed,
        /// tühjendab väljad (va eesnimi, kus on Guid), sisestab teise andmekomplekti ja salvestab.
        /// Seejärel avab sama õppejõu detailvaate ja kontrollib, et seal on uued andmed, vanu andmeid
        /// enam ei ole ja õppejõu id jäi samaks.
        /// </summary>
        [Fact]
        [Order(4)]
        public void Should_UpdateDetailsOfUniqueLecturer_WithNewData()
        {
            /* Ülesseade */
            IWebDriver driver = TestSetup_NavigateToIndex();
            var tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            int rowsBefore = rowsOfTable.Count;
            bool isObjectFound = false;

            /* tegevus */
            foreach (var row in rowsOfTable)
            {
                var nameElement = row.FindElement(By.Id("IndexTableLecturerFirstName")).Text;
                if (nameElement == thisRunGuid)
                {
                    isObjectFound = true;
                    var clickAction = row
                        .FindElement(By.Id("IndexTableLecturerActions"))
                        .FindElement(By.Id("IndexTableLecturerUpdate_BTN"));
                    clickAction.Click();
                    WaitForNewPage(clickAction); //ootame, kuni uus leht laeb
                    break;
                }
            }
            Thread.Sleep(500);
            //vormis peavad enne muutmist olema vanad andmed
            string updateHeading = driver.FindElement(By.TagName("h1")).Text;
            string updateUrl = driver.Url;
            string oldLastNameInForm = driver.FindElement(By.Id("CU_Lecturer_LastName")).GetDomProperty("value");
            string oldQualificationsInForm = driver.FindElement(By.Id("CU_Lecturer_Qualifications")).GetDomProperty("value");
            DataClear_CreateUpdate_SaveName(driver);
            DataEntry_Lecturer(driver, true, false); //teine andmekomplekt, eesnime ei muuda
            IWebElement updateclick = driver.FindElement(By.Id("CU_Update_BTN"));
            updateclick.Click();
            WaitForNewPage(updateclick); //ootame, kuni uus leht laeb
            Thread.Sleep(1000); //ootame, kuni Index leht laeb
            string urlAfterUpdate = driver.Url;

            /* kontroll */
            //otsime õppejõu uuesti üles ja avame tema detailvaate
            tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            int rowsAfter = rowsOfTable.Count;
            bool isObjectFoundAfterUpdate = false;
            foreach (var row in rowsOfTable)
            {
                var nameElement = row.FindElement(By.Id("IndexTableLecturerFirstName")).Text;
                if (nameElement == thisRunGuid)
                {
                    isObjectFoundAfterUpdate = true;
                    var clickAction = row
                        .FindElement(By.Id("IndexTableLecturerActions"))
                        .FindElement(By.Id("IndexTableLecturerDetails_BTN"));
                    clickAction.Click();
                    WaitForNewPage(clickAction); //ootame, kuni uus leht laeb
                    break;
                }
            }
            string details_id = driver.FindElement(By.Id("DD_LecturerGuid")).Text;
            string details_firstName = driver.FindElement(By.Id("DD_LecturerFirstName")).Text; //unikaalne id
            string details_lastName = driver.FindElement(By.Id("DD_LecturerLastName")).Text;
            string details_qualifications = driver.FindElement(By.Id("DD_LecturerQualifications")).Text;
            Thread.Sleep(1000);
            driver.Quit();

            Assert.True(isObjectFound);                                    //muudetav õppejõud leiti
            Assert.Equal("Muuda õppejõudu", updateHeading);                //avati muutmise (mitte lisamise) vorm
            Assert.Equal("Oleinik", oldLastNameInForm);                       //vormis olid enne vanad andmed
            Assert.Equal("Inglise keel C1, IELTS", oldQualificationsInForm);
            Assert.EndsWith("/Lecturers", urlAfterUpdate);                 //salvestamise järel suunati nimekirja
            Assert.Equal(rowsBefore, rowsAfter);                           //muutmine ei lisanud uut rida
            Assert.True(isObjectFoundAfterUpdate);                         //õppejõud on pärast muutmist alles
            Assert.Contains("/Lecturers/Update/" + details_id, updateUrl); //muudeti sama id-ga õppejõudu
            Assert.Equal(thisRunGuid, details_firstName);                  //eesnimi (Guid) ei muutunud
            Assert.Equal("Speek", details_lastName);                        //perenimi muudeti
            Assert.Equal("Saksa keel B2, eksamineerija", details_qualifications); //kvalifikatsioonid muudeti
            Assert.NotEqual("Oleinik", details_lastName);                     //vana perenime enam ei ole
        }

        /// <summary>
        /// DELETE test: avab varem lisatud õppejõu kustutamise vaate, kontrollib et seal on õige õppejõud
        /// ja kustutamise nupp, kinnitab kustutamise ja kontrollib, et suunati nimekirja, tabelist kadus
        /// täpselt üks rida ja seda õppejõudu ei ole enam nimekirjas.
        /// Test käivitatakse viimasena, sest kustutab õppejõu, mida teised testid kasutavad.
        /// </summary>
        [Fact]
        [Order(10)]
        public void Should_DeleteUniqueLecturer_FromDeleteView()
        {
            /* ülesseade */
            IWebDriver driver = TestSetup_NavigateToIndex();
            var tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            int rowsBefore = rowsOfTable.Count;
            bool isObjectFound = false;

            /* tegevus */
            foreach (var row in rowsOfTable)
            {
                var nameElement = row.FindElement(By.Id("IndexTableLecturerFirstName")).Text;
                if (nameElement == thisRunGuid)
                {
                    isObjectFound = true;
                    var clickAction = row
                        .FindElement(By.Id("IndexTableLecturerActions"))
                        .FindElement(By.Id("IndexTableLecturerDelete_BTN"));
                    clickAction.Click();
                    WaitForNewPage(clickAction); //ootame, kuni uus leht laeb
                    break;
                }
            }
            Thread.Sleep(1000);
            string delete_heading = driver.FindElement(By.TagName("h1")).Text;
            string delete_id = driver.FindElement(By.Id("DD_LecturerGuid")).Text;
            string delete_firstName = driver.FindElement(By.Id("DD_LecturerFirstName")).Text;
            string deleteUrl = driver.Url;
            IWebElement clickDelete = driver.FindElement(By.Id("DeleteDelete_BTN"));
            clickDelete.Click();
            WaitForNewPage(clickDelete); //ootame, kuni uus leht laeb
            Thread.Sleep(1000); //ootame, kuni Index leht laeb
            string urlAfterDelete = driver.Url;

            //kontroll
            bool isObjectFoundAfterDelete = false;
            tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            int rowsAfter = rowsOfTable.Count;
            foreach (var row in rowsOfTable)
            {
                var nameElement = row.FindElement(By.Id("IndexTableLecturerFirstName")).Text;
                if (nameElement == thisRunGuid)
                {
                    isObjectFoundAfterDelete = true;
                    break;
                }
            }
            driver.Quit();

            Assert.True(isObjectFound);                                    //enne kustutamist oli õppejõud olemas
            Assert.Equal("Kustuta", delete_heading);                       //avati kustutamise vaade
            Assert.Contains("/Lecturers/Delete/" + delete_id, deleteUrl);
            Assert.Equal(thisRunGuid, delete_firstName);                   //kustutamise vaates oli õige õppejõud
            Assert.EndsWith("/Lecturers", urlAfterDelete);                 //kustutamise järel suunati nimekirja
            Assert.Equal(rowsBefore - 1, rowsAfter);                       //tabelist kadus täpselt üks rida
            Assert.False(isObjectFoundAfterDelete);                        //õppejõudu ei ole enam nimekirjas
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
