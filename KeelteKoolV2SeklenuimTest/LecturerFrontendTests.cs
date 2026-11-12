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
