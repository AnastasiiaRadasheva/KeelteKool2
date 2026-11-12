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

        //eof

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
