using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace KeelteKoolV2SeleniumTesting
{
    //Õpilaste (Students) kasutajaliidese testid. Testid käivitatakse kindlas järjekorras (Order),
    //sest esimene test loob õpilase, mida järgmised testid vaatavad, muudavad ja lõpuks kustutavad.
    //NB! Enne testide käivitamist peab veebileht töötama aadressil http://localhost:5234 (Ctrl+F5).
    [TestCaseOrderer("KeelteKoolV2SeleniumTesting.PriorityOrderer", "KeelteKoolV2SeleniumTesting")]
    public class StudentFrontendTests
    {
        private const string BaseUrl = "http://localhost:5234/";

        /// <summary>
        /// Ülesseade meetod: avab Chrome brauseri, läheb avalehele ja vajutab menüüs lingile "Õpilased".
        /// </summary>
        /// <returns>Draiver, mis on õpilaste nimekirja (Index) lehel</returns>
        private static IWebDriver TestSetup_NavigateToStudents()
        {
            IWebDriver driver = new ChromeDriver();
            driver.Manage().Window.Maximize(); //suur aken, et footer ei kataks nuppe
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5); //ootame, kuni element lehele ilmub
            driver.Url = BaseUrl;
            IWebElement studentsLink = driver.FindElement(By.LinkText("Õpilased"));
            studentsLink.Click();
            WaitForNewPage(studentsLink);
            return driver;
        }

        /// <summary>
        /// Ootab pärast klikki, kuni vana leht kaob (element muutub "stale") ehk uus leht on laetud.
        /// </summary>
        /// <param name="clickedElement">Element, millele just klikiti</param>
        private static void WaitForNewPage(IWebElement clickedElement)
        {
            for (int i = 0; i < 100; i++) //kõige rohkem 10 sekundit
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
