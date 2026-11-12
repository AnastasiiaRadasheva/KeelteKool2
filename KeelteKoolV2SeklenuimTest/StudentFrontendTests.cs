using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using Xunit;

namespace KeelteKoolV2SeleniumTesting
{
    //Õpilaste (Students) kasutajaliidese testid. Testid käivitatakse kindlas järjekorras (Order),
    //sest esimene test loob õpilase, mida järgmised testid vaatavad, muudavad ja lõpuks kustutavad.
    //NB! Enne testide käivitamist peab veebileht töötama aadressil http://localhost:5234 (Ctrl+F5).
    [TestCaseOrderer("KeelteKoolV2SeleniumTesting.PriorityOrderer", "KeelteKoolV2SeleniumTesting")]
    public class StudentFrontendTests
    {
        //selle testikäivituse õpilase unikaalne Guid, mida hoitakse õpilase eesnimes
        public static string thisRunGuid { get; set; } = "";

        //Zen brauser on Firefoxi põhine, seega saab seda juhtida FirefoxDriveriga
        private const string ZenBrowserPath = @"C:\Program Files\Zen Browser\zen.exe";

        /// <summary>
        /// CREATE test: avab õpilaste nimekirja, vajutab "Lisa uus õpilane", sisestab unikaalse Guidiga
        /// eesnime ja ülejäänud andmed ning salvestab. Kontrollib, et uus õpilane on nimekirja tabelis
        /// ja tabelis on täpselt need andmed, mis vormi sisestati.
        /// </summary>
        [Fact]
        [Order(5)]
        public void Should_NavigateToIndex_AddStudentWithUniqueData_ReturnToIndex()
        {
            /* Ülesseade */
            IWebDriver driver = TestSetup_NavigateToIndex();

            /* tegevus */
            IWebElement indexCreateButton = driver.FindElement(By.Id("IndexCreateNewStudent_BTN"));
            indexCreateButton.Click(); //vajutame lisamise nupule
            Thread.Sleep(1000);

            string uniqueId = DataEntry_Student(driver); //sisestame andmed vormi
            thisRunGuid = uniqueId;

            IWebElement createCreateButton = driver.FindElement(By.Id("CU_Create_BTN"));
            createCreateButton.Click(); //vajutame vormis salvestamise nupule
            Thread.Sleep(1000);

            /* kontroll */
            //kogume andmed tabelist
            var tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0); //eemaldame päise rea
            bool isObjectFound = false;
            string studentLastNameData = "";
            string studentEmailData = "";
            string studentLevelData = "";
            foreach (var row in rowsOfTable)
            {
                if (row.FindElement(By.Id("IndexTableStudentFirstName")).Text == thisRunGuid)
                {
                    isObjectFound = true;
                    studentLastNameData = row.FindElement(By.Id("IndexTableStudentLastName")).Text;
                    studentEmailData = row.FindElement(By.Id("IndexTableStudentEmail")).Text;
                    studentLevelData = row.FindElement(By.Id("IndexTableStudentLanguageLevel")).Text;
                }
            }
            string urlAfterSave = driver.Url;
            //sulgeme akna
            Thread.Sleep(500);
            driver.Quit();

            //assertime
            Assert.True(isObjectFound);                                      //uus õpilane on tabelis
            Assert.EndsWith("/Students", urlAfterSave);                      //salvestamise järel suunati nimekirja
            Assert.Equal("Seleniumitest", studentLastNameData);
            Assert.Equal("selenium.opilane@test.ee", studentEmailData);
            Assert.Equal("B1", studentLevelData);
            Assert.True(Guid.TryParse(thisRunGuid, out _));                  //eesnimes on korrektne Guid
        }

        /// <summary>
        /// READ test: leiab eelmises testis lisatud õpilase nimekirjast, avab nupuga "Andmed" tema detailvaate
        /// ja kontrollib, et detailvaates on sama õpilase õiged andmed. Lõpuks vajutab "Tagasi"
        /// ja kontrollib, et jõuti tagasi nimekirja.
        /// </summary>
        [Fact]
        [Order(6)]
        public void Should_NavigateToDetailsOfAStudent_WithPreviouslyUniqueData()
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
                var nameElement = row.FindElement(By.Id("IndexTableStudentFirstName")).Text;
                if (nameElement == thisRunGuid)
                {
                    isObjectFound = true;
                    var clickAction = row
                        .FindElement(By.Id("IndexTableStudentActions"))
                        .FindElement(By.Id("IndexTableStudentDetails_BTN"));
                    clickAction.Click();
                    break;
                }
            }
            Thread.Sleep(1000);
            //kogume detailvaate andmed
            string details_id = driver.FindElement(By.Id("DD_StudentGuid")).Text;
            string details_firstName = driver.FindElement(By.Id("DD_StudentFirstName")).Text; //omab testimise jaoks vajalikku Guidi
            string details_lastName = driver.FindElement(By.Id("DD_StudentLastName")).Text;
            string details_email = driver.FindElement(By.Id("DD_StudentEmail")).Text;
            string details_level = driver.FindElement(By.Id("DD_StudentLanguageLevel")).Text;
            string detailsUrl = driver.Url;
            IWebElement goBack = driver.FindElement(By.Id("DetailsBack_BTN"));
            goBack.Click();
            Thread.Sleep(1000);
            string urlAfterBack = driver.Url;
            driver.Quit();

            /* kontroll */
            Assert.True(isObjectFound);                                      //õpilane leiti nimekirjast
            Assert.True(Guid.TryParse(details_id, out _));                   //detailvaates on korrektne id
            Assert.Contains("/Students/Details/" + details_id, detailsUrl);  //avati just selle õpilase leht
            Assert.Equal(thisRunGuid, details_firstName);
            Assert.Equal("Seleniumitest", details_lastName);
            Assert.Equal("selenium.opilane@test.ee", details_email);
            Assert.Equal("B1", details_level);
            Assert.EndsWith("/Students", urlAfterBack);                      //"Tagasi" viis nimekirja
        }

        /// <summary>
        /// UPDATE test: avab varem lisatud õpilase muutmise vormi, tühjendab väljad (va eesnimi, kus on Guid),
        /// sisestab teise andmekomplekti ja salvestab. Seejärel avab sama õpilase detailvaate ja kontrollib,
        /// et seal on uued andmed ning vanu andmeid enam ei ole.
        /// </summary>
        [Fact]
        [Order(7)]
        public void Should_UpdateDetailsOfUniqueStudent_WithNewData()
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
                var nameElement = row.FindElement(By.Id("IndexTableStudentFirstName")).Text;
                if (nameElement == thisRunGuid)
                {
                    isObjectFound = true;
                    var clickAction = row
                        .FindElement(By.Id("IndexTableStudentActions"))
                        .FindElement(By.Id("IndexTableStudentUpdate_BTN"));
                    clickAction.Click();
                    break;
                }
            }
            Thread.Sleep(1000);
            //vormis peavad enne muutmist olema vanad andmed
            string oldLastNameInForm = driver.FindElement(By.Id("CU_Student_LastName")).GetDomProperty("value");
            DataClear_CreateUpdate_SaveName(driver);
            DataEntry_Student(driver, true, false); //teine andmekomplekt, eesnime ei muuda
            IWebElement updateClick = driver.FindElement(By.Id("CU_Update_BTN"));
            updateClick.Click();
            Thread.Sleep(1000);
            IWebElement goBack = driver.FindElement(By.Id("CU_Cancel_BTN"));
            goBack.Click();
            Thread.Sleep(1000);

            /* kontroll */
            //otsime õpilase uuesti üles ja avame tema detailvaate
            tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            bool isObjectFoundAfterUpdate = false;
            foreach (var row in rowsOfTable)
            {
                var nameElement = row.FindElement(By.Id("IndexTableStudentFirstName")).Text;
                if (nameElement == thisRunGuid)
                {
                    isObjectFoundAfterUpdate = true;
                    var clickAction = row
                        .FindElement(By.Id("IndexTableStudentActions"))
                        .FindElement(By.Id("IndexTableStudentDetails_BTN"));
                    clickAction.Click();
                    break;
                }
            }
            Thread.Sleep(1000);
            string details_firstName = driver.FindElement(By.Id("DD_StudentFirstName")).Text;
            string details_lastName = driver.FindElement(By.Id("DD_StudentLastName")).Text;
            string details_email = driver.FindElement(By.Id("DD_StudentEmail")).Text;
            string details_level = driver.FindElement(By.Id("DD_StudentLanguageLevel")).Text;
            driver.Quit();

            Assert.True(isObjectFound);                                      //muudetav õpilane leiti
            Assert.True(isObjectFoundAfterUpdate);                           //õpilane on pärast muutmist alles
            Assert.Equal("Seleniumitest", oldLastNameInForm);                //vormis olid enne vanad andmed
            Assert.Equal(thisRunGuid, details_firstName);                    //eesnimi (Guid) ei muutunud
            Assert.Equal("Seleniumitest-Muudetud", details_lastName);        //perenimi muudeti
            Assert.Equal("muudetud.opilane@test.ee", details_email);         //e-post muudeti
            Assert.Equal("C1", details_level);                               //keeletase muudeti
            Assert.NotEqual("selenium.opilane@test.ee", details_email);      //vana e-posti enam ei ole
        }

        // ---------------- ise välja mõeldud kasutajaliidese testid ----------------

        /// <summary>
        /// Oma test: detailvaates olev nupp "Muuda" peab avama sama õpilase muutmise vormi,
        /// kus väljad on täidetud täpselt nende andmetega, mida detailvaade näitas.
        /// </summary>
        [Fact]
        [Order(8)]
        public void Should_OpenUpdateForm_FromDetailsView_WithSameStudentData()
        {
            /* ülesseade */
            IWebDriver driver = TestSetup_NavigateToIndex();
            var tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            bool isObjectFound = false;
            foreach (var row in rowsOfTable)
            {
                var nameElement = row.FindElement(By.Id("IndexTableStudentFirstName")).Text;
                if (nameElement == thisRunGuid)
                {
                    isObjectFound = true;
                    var clickAction = row
                        .FindElement(By.Id("IndexTableStudentActions"))
                        .FindElement(By.Id("IndexTableStudentDetails_BTN"));
                    clickAction.Click();
                    break;
                }
            }
            Thread.Sleep(1000);
            string details_id = driver.FindElement(By.Id("DD_StudentGuid")).Text;
            string details_lastName = driver.FindElement(By.Id("DD_StudentLastName")).Text;
            string details_email = driver.FindElement(By.Id("DD_StudentEmail")).Text;
            string details_level = driver.FindElement(By.Id("DD_StudentLanguageLevel")).Text;

            /* tegevus */
            IWebElement detailsUpdate = driver.FindElement(By.Id("DetailsUpdate_BTN"));
            detailsUpdate.Click();
            Thread.Sleep(1000);
            string form_firstName = driver.FindElement(By.Id("CU_Student_FirstName")).GetDomProperty("value");
            string form_lastName = driver.FindElement(By.Id("CU_Student_LastName")).GetDomProperty("value");
            string form_email = driver.FindElement(By.Id("CU_Student_Email")).GetDomProperty("value");
            string form_level = driver.FindElement(By.Id("CU_Student_LanguageLevel")).GetDomProperty("value");
            string formUrl = driver.Url;
            int updateButtons = driver.FindElements(By.Id("CU_Update_BTN")).Count;
            driver.Quit();

            /* kontroll */
            Assert.True(isObjectFound);
            Assert.Contains("/Students/Update/" + details_id, formUrl);       //avati sama õpilase muutmise vorm
            Assert.Equal(1, updateButtons);                                  //vormis on "Muuda" nupp, mitte "Lisa"
            Assert.Equal(thisRunGuid, form_firstName);
            Assert.Equal(details_lastName, form_lastName);
            Assert.Equal(details_email, form_email);
            Assert.Equal(details_level, form_level);
        }

        /// <summary>
        /// Oma (negatiivne) test: kui kustutamise vaates vajutatakse "Tühista", ei tohi õpilast kustutada.
        /// Kontrollib, et jõuti tagasi nimekirja, õpilaste arv ei muutunud ja õpilane on endiselt tabelis.
        /// </summary>
        [Fact]
        [Order(9)]
        public void ShouldNot_DeleteStudent_WhenDeleteIsCancelled()
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
                var nameElement = row.FindElement(By.Id("IndexTableStudentFirstName")).Text;
                if (nameElement == thisRunGuid)
                {
                    isObjectFound = true;
                    var clickAction = row
                        .FindElement(By.Id("IndexTableStudentActions"))
                        .FindElement(By.Id("IndexTableStudentDelete_BTN"));
                    clickAction.Click();
                    break;
                }
            }
            Thread.Sleep(1000);
            int deleteButtons = driver.FindElements(By.Id("DeleteDelete_BTN")).Count;
            IWebElement cancelDelete = driver.FindElement(By.Id("DeleteCancel_BTN"));
            cancelDelete.Click();
            Thread.Sleep(1000);

            /* kontroll */
            string urlAfterCancel = driver.Url;
            tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            bool isObjectFoundAfterCancel = false;
            foreach (var row in rowsOfTable)
            {
                if (row.FindElement(By.Id("IndexTableStudentFirstName")).Text == thisRunGuid)
                {
                    isObjectFoundAfterCancel = true;
                }
            }
            int rowsAfter = rowsOfTable.Count;
            driver.Quit();

            Assert.True(isObjectFound);                                      //õpilane oli olemas
            Assert.Equal(1, deleteButtons);                                  //kustutamise vaates oli kinnitusnupp
            Assert.EndsWith("/Students", urlAfterCancel);                    //"Tühista" viis nimekirja
            Assert.True(isObjectFoundAfterCancel);                           //õpilane on endiselt tabelis
            Assert.Equal(rowsBefore, rowsAfter);                             //õpilaste arv ei muutunud
        }

        /// <summary>
        /// Oma (negatiivne) test: kui e-posti aadressis puudub @ märk, ei tohi õpilast lisada.
        /// Kontrollib, et kasutaja jääb lisamise vormile, näeb veateadet, sisestatud e-post ei kao
        /// ja sellise Guidiga õpilast ei teki nimekirja.
        /// </summary>
        [Fact]
        [Order(10)]
        public void ShouldNot_AddStudent_WhenEmailIsInvalid()
        {
            /* Ülesseade */
            IWebDriver driver = TestSetup_NavigateToIndex();
            IWebElement indexCreateButton = driver.FindElement(By.Id("IndexCreateNewStudent_BTN"));
            indexCreateButton.Click();
            Thread.Sleep(1000);

            /* tegevus */
            string invalidGuid = DataEntry_Student(driver); //täidame vormi korrektsete andmetega
            IWebElement emailField = driver.FindElement(By.Id("CU_Student_Email"));
            emailField.Clear();
            emailField.SendKeys("vale.email.test.ee"); //asendame e-posti vigasega
            IWebElement createCreateButton = driver.FindElement(By.Id("CU_Create_BTN"));
            createCreateButton.Click();
            Thread.Sleep(1000);

            string errorText = driver.FindElement(By.CssSelector(".validation-summary-errors")).Text;
            string emailInForm = driver.FindElement(By.Id("CU_Student_Email")).GetDomProperty("value");
            int createButtons = driver.FindElements(By.Id("CU_Create_BTN")).Count;
            IWebElement goBack = driver.FindElement(By.Id("CU_Cancel_BTN"));
            goBack.Click();
            Thread.Sleep(1000);

            /* kontroll */
            var tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            bool isObjectFound = false;
            foreach (var row in rowsOfTable)
            {
                if (row.FindElement(By.Id("IndexTableStudentFirstName")).Text == invalidGuid)
                {
                    isObjectFound = true;
                }
            }
            driver.Quit();

            Assert.Equal(1, createButtons);                                  //kasutaja jäi lisamise vormile
            Assert.Contains("ei ole korrektsed", errorText);                 //kuvati veateade
            Assert.Equal("vale.email.test.ee", emailInForm);                 //sisestatud e-post ei kadunud
            Assert.DoesNotContain("@", emailInForm);                         //e-post oli tõesti ilma @ märgita
            Assert.False(isObjectFound);                                     //õpilast ei lisatud nimekirja
        }

        /// <summary>
        /// Oma (negatiivne) test: kui ees- ja perenimi jäetakse tühjaks, ei tohi õpilast lisada.
        /// Kontrollib, et vorm ei salvestu, mõlema nimevälja juures on veateade
        /// ja nimekirja ei tule ühtegi uut õpilast.
        /// </summary>
        [Fact]
        [Order(11)]
        public void ShouldNot_AddStudent_WhenNamesAreEmpty()
        {
            /* Ülesseade */
            IWebDriver driver = TestSetup_NavigateToIndex();
            int rowsBefore = driver.FindElement(By.Id("IndexTable")).FindElements(By.TagName("tr")).Count - 1;
            IWebElement indexCreateButton = driver.FindElement(By.Id("IndexCreateNewStudent_BTN"));
            indexCreateButton.Click();
            Thread.Sleep(1000);

            /* tegevus */
            DataEntry_Student(driver, false, false); //eesnime ei täida
            driver.FindElement(By.Id("CU_Student_LastName")).Clear(); //perenime tühjendame
            IWebElement createCreateButton = driver.FindElement(By.Id("CU_Create_BTN"));
            createCreateButton.Click();
            Thread.Sleep(1000);

            string firstNameError = driver.FindElement(By.CssSelector("span[data-valmsg-for='FirstName']")).Text;
            string lastNameError = driver.FindElement(By.CssSelector("span[data-valmsg-for='LastName']")).Text;
            int createButtons = driver.FindElements(By.Id("CU_Create_BTN")).Count;
            IWebElement goBack = driver.FindElement(By.Id("CU_Cancel_BTN"));
            goBack.Click();
            Thread.Sleep(1000);

            /* kontroll */
            int rowsAfter = driver.FindElement(By.Id("IndexTable")).FindElements(By.TagName("tr")).Count - 1;
            string urlAfterBack = driver.Url;
            driver.Quit();

            Assert.Equal(1, createButtons);                                  //vorm ei salvestunud
            Assert.False(string.IsNullOrWhiteSpace(firstNameError));         //eesnime juures on veateade
            Assert.False(string.IsNullOrWhiteSpace(lastNameError));          //perenime juures on veateade
            Assert.EndsWith("/Students", urlAfterBack);                      //"Tagasi" viis nimekirja
            Assert.Equal(rowsBefore, rowsAfter);                             //uut õpilast ei lisatud
        }

        /// <summary>
        /// Oma (negatiivne) test: kui kasutaja täidab lisamise vormi, aga vajutab "Tagasi" ilma salvestamata,
        /// ei tohi õpilast lisada. Kontrollib, et andmed olid vormis, jõuti nimekirja,
        /// õpilaste arv ei muutunud ja sellise Guidiga õpilast tabelis ei ole.
        /// </summary>
        [Fact]
        [Order(12)]
        public void ShouldNot_AddStudent_WhenCreateIsCancelled()
        {
            /* Ülesseade */
            IWebDriver driver = TestSetup_NavigateToIndex();
            int rowsBefore = driver.FindElement(By.Id("IndexTable")).FindElements(By.TagName("tr")).Count - 1;
            IWebElement indexCreateButton = driver.FindElement(By.Id("IndexCreateNewStudent_BTN"));
            indexCreateButton.Click();
            Thread.Sleep(1000);

            /* tegevus */
            string cancelledGuid = DataEntry_Student(driver); //täidame vormi, aga ei salvesta
            string firstNameInForm = driver.FindElement(By.Id("CU_Student_FirstName")).GetDomProperty("value");
            IWebElement goBack = driver.FindElement(By.Id("CU_Cancel_BTN"));
            goBack.Click();
            Thread.Sleep(1000);

            /* kontroll */
            var tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            bool isObjectFound = false;
            foreach (var row in rowsOfTable)
            {
                if (row.FindElement(By.Id("IndexTableStudentFirstName")).Text == cancelledGuid)
                {
                    isObjectFound = true;
                }
            }
            string urlAfterBack = driver.Url;
            driver.Quit();

            Assert.EndsWith("/Students", urlAfterBack);                      //"Tagasi" viis nimekirja
            Assert.False(isObjectFound);                                     //sellise Guidiga õpilast ei ole
        }

        /// <summary>
        /// DELETE test: avab varem lisatud õpilase kustutamise vaate, kontrollib et seal on õige õpilane,
        /// kinnitab kustutamise ja kontrollib, et õpilast ei ole enam nimekirjas ega saa avada tema detailvaadet.
        /// Test käivitatakse viimasena, sest kustutab õpilase, mida teised testid kasutavad.
        /// </summary>
        [Fact]
        [Order(13)]
        public void Should_DeleteUniqueStudent_FromDeleteView()
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
                var nameElement = row.FindElement(By.Id("IndexTableStudentFirstName")).Text;
                if (nameElement == thisRunGuid)
                {
                    isObjectFound = true;
                    var clickAction = row
                        .FindElement(By.Id("IndexTableStudentActions"))
                        .FindElement(By.Id("IndexTableStudentDelete_BTN"));
                    clickAction.Click();
                    break;
                }
            }
            Thread.Sleep(1000);
            string delete_id = driver.FindElement(By.Id("DD_StudentGuid")).Text;
            string delete_firstName = driver.FindElement(By.Id("DD_StudentFirstName")).Text;
            IWebElement clickDelete = driver.FindElement(By.Id("DeleteDelete_BTN"));
            clickDelete.Click();
            Thread.Sleep(1000);
            string urlAfterDelete = driver.Url;

            //kontroll
            bool isObjectFoundAfterDelete = false;
            tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            foreach (var row in rowsOfTable)
            {
                var nameElement = row.FindElement(By.Id("IndexTableStudentFirstName")).Text;
                if (nameElement == thisRunGuid)
                {
                    isObjectFoundAfterDelete = true;
                    break;
                }
            }
            //proovime kustutatud õpilase detailvaadet otse aadressiga avada
            driver.Url = "http://localhost:5234/Students/Details/" + delete_id;
            Thread.Sleep(1000);
            int detailsFieldsFound = driver.FindElements(By.Id("DD_StudentGuid")).Count;
            driver.Quit();

            Assert.True(isObjectFound);                                      //enne kustutamist oli õpilane olemas
            Assert.Equal(thisRunGuid, delete_firstName);                     //kustutamise vaates oli õige õpilane
            Assert.True(Guid.TryParse(delete_id, out _));
            Assert.EndsWith("/Students", urlAfterDelete);                    //kustutamise järel suunati nimekirja
            Assert.False(isObjectFoundAfterDelete);                          //õpilast ei ole enam nimekirjas
            Assert.Equal(0, detailsFieldsFound);                             //kustutatud õpilase detailvaadet ei ole
        }

        /// <summary>
        /// Puhastab Create/Update vormi kõik andmeväljad meetodile kaasaantud draiveri sessioonis.
        /// </summary>
        /// <param name="driver">Hetkel käsitletav draiver</param>
        private void DataClear_CreateUpdate(IWebDriver driver)
        {
            IWebElement createEntryFirstName = driver.FindElement(By.Id("CU_Student_FirstName"));
            createEntryFirstName.Clear();
            IWebElement createEntryLastName = driver.FindElement(By.Id("CU_Student_LastName"));
            createEntryLastName.Clear();
            IWebElement createEntryEmail = driver.FindElement(By.Id("CU_Student_Email"));
            createEntryEmail.Clear();
            IWebElement createEntryLanguageLevel = driver.FindElement(By.Id("CU_Student_LanguageLevel"));
            createEntryLanguageLevel.Clear();
        }

        /// <summary>
        /// Puhastab Create/Update vormi andmeväljad, välja arvatud eesnime välja,
        /// sest eesnimes hoitakse testimiseks vajalikku unikaalset Guidi.
        /// </summary>
        /// <param name="driver">Hetkel käsitletav draiver</param>
        private void DataClear_CreateUpdate_SaveName(IWebDriver driver)
        {
            IWebElement createEntryLastName = driver.FindElement(By.Id("CU_Student_LastName"));
            createEntryLastName.Clear();
            IWebElement createEntryEmail = driver.FindElement(By.Id("CU_Student_Email"));
            createEntryEmail.Clear();
            IWebElement createEntryLanguageLevel = driver.FindElement(By.Id("CU_Student_LanguageLevel"));
            createEntryLanguageLevel.Clear();
        }

        /// <summary>
        /// Meetod mis testimise eesmärgil sisestab õpilase vormi andmed, on võimalik konfigureerida.
        /// </summary>
        /// <param name="driver">WebDriver mida hetkel test rakendab</param>
        /// <param name="isOneOrTwo">Muutuja mis valib kahe erineva andmekomplekti vahel</param>
        /// <param name="updateName">Muutuja mis ütleb kas eesnime on vaja täita (eesnimes hoitakse unikaalset Guidi)</param>
        /// <returns>Tagastab eesnimesse pandud Guidi stringi kujul</returns>
        private static string DataEntry_Student(IWebDriver driver, bool isOneOrTwo = false, bool updateName = true)
        {
            Guid generatedId = Guid.NewGuid();
            if (updateName)
            {
                IWebElement createEntryFirstName = driver.FindElement(By.Id("CU_Student_FirstName"));
                createEntryFirstName.SendKeys(generatedId.ToString());
            }

            if (!isOneOrTwo)
            {
                IWebElement createEntryLastName = driver.FindElement(By.Id("CU_Student_LastName"));
                createEntryLastName.SendKeys("Seleniumitest");
                IWebElement createEntryEmail = driver.FindElement(By.Id("CU_Student_Email"));
                createEntryEmail.SendKeys("selenium.opilane@test.ee");
                IWebElement createEntryLanguageLevel = driver.FindElement(By.Id("CU_Student_LanguageLevel"));
                createEntryLanguageLevel.SendKeys("B1");
            }
            else
            {
                IWebElement createEntryLastName = driver.FindElement(By.Id("CU_Student_LastName"));
                createEntryLastName.SendKeys("Seleniumitest-Muudetud");
                IWebElement createEntryEmail = driver.FindElement(By.Id("CU_Student_Email"));
                createEntryEmail.SendKeys("muudetud.opilane@test.ee");
                IWebElement createEntryLanguageLevel = driver.FindElement(By.Id("CU_Student_LanguageLevel"));
                createEntryLanguageLevel.SendKeys("C1");
            }
            return generatedId.ToString();
        }

        /// <summary>
        /// Tekitab brauserit juhtiva draiveri: kui arvutis on Zen brauser, kasutatakse FirefoxDriverit
        /// koos Zeniga, muidu kasutatakse Chrome brauserit.
        /// </summary>
        /// <returns>Brauserit juhtiv draiver</returns>
        private static IWebDriver CreateDriver()
        {
            //geckodriver (Firefoxi draiver) laetakse Seleniumi poolt kausta %USERPROFILE%\.cache\selenium\geckodriver
            string geckoFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache", "selenium", "geckodriver");
            string? geckoDriver = Directory.Exists(geckoFolder)
                ? Directory.GetFiles(geckoFolder, "geckodriver.exe", SearchOption.AllDirectories).OrderByDescending(x => x).FirstOrDefault()
                : null;

            if (File.Exists(ZenBrowserPath) && geckoDriver != null)
            {
                FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(Path.GetDirectoryName(geckoDriver));
                FirefoxOptions options = new FirefoxOptions();
                options.BinaryLocation = ZenBrowserPath;
                options.SetPreference("zen.welcome-screen.seen", true); //muidu katab Zeni tervitusaken lehe sisu
                return new FirefoxDriver(service, options);
            }
            return new ChromeDriver();
        }

        /// <summary>
        /// Ülesseade meetod mis tekitab draiveri, navigeerib aadressile ja vajutab Õpilaste lingile.
        /// </summary>
        /// <returns>Tagastab testisiseselt kasutatava draiveri</returns>
        private static IWebDriver TestSetup_NavigateToIndex()
        {
            IWebDriver driver = CreateDriver(); //brauserit juhtiv ja seal tegutsev draiver
            driver.Manage().Window.Maximize(); //suur aken, et footer ei kataks nuppe
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5); //ootame, kuni element lehele ilmub
            driver.Url = "http://localhost:5234/"; //navigeeritav aadress
            Thread.Sleep(2000);
            IWebElement navigateToStudents = driver.FindElement(By.LinkText("Õpilased")); //otsime testitava osa lingi
            navigateToStudents.Click(); //simuleeritud tegevus elemendiga, antud juhul klikk
            Thread.Sleep(1000);
            return driver;
        }
    }
}
