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

        //selle testikäivituse õpilase eesnimi (unikaalne Guid) ja andmebaasi id,
        //mida kasutavad kõik järgnevad testid
        public static string thisRunFirstName { get; set; } = "";
        public static string thisRunStudentId { get; set; } = "";

        //andmed, millega õpilane luuakse
        private const string CreateLastName = "Automaattest";
        private const string CreateLevel = "B1";
        //andmed, millega õpilast muudetakse
        private const string UpdateLastName = "Automaattest-Muudetud";
        private const string UpdateEmail = "muudetud.selenium@test.ee";
        private const string UpdateLevel = "C1";

        /// <summary>
        /// Sisestab Create/Update vormi väljadesse kaasaantud andmed.
        /// </summary>
        /// <param name="driver">Hetkel kasutatav draiver</param>
        /// <param name="firstName">Eesnimi, null korral välja ei muudeta</param>
        /// <param name="lastName">Perenimi</param>
        /// <param name="email">E-posti aadress</param>
        /// <param name="level">Keeletase</param>
        private static void DataEntry_Student(IWebDriver driver, string? firstName, string lastName, string email, string level)
        {
            if (firstName != null)
            {
                driver.FindElement(By.Id("CU_Student_FirstName")).SendKeys(firstName);
            }
            driver.FindElement(By.Id("CU_Student_LastName")).SendKeys(lastName);
            driver.FindElement(By.Id("CU_Student_Email")).SendKeys(email);
            driver.FindElement(By.Id("CU_Student_LanguageLevel")).SendKeys(level);
        }

        /// <summary>
        /// Tühjendab vormi väljad peale eesnime, sest eesnimes hoitakse testi jaoks vajalikku unikaalset Guidi.
        /// </summary>
        /// <param name="driver">Hetkel kasutatav draiver</param>
        private static void DataClear_StudentExceptFirstName(IWebDriver driver)
        {
            driver.FindElement(By.Id("CU_Student_LastName")).Clear();
            driver.FindElement(By.Id("CU_Student_Email")).Clear();
            driver.FindElement(By.Id("CU_Student_LanguageLevel")).Clear();
        }

        /// <summary>
        /// Otsib õpilaste tabelist rea, mille eesnimi on kaasaantud väärtus.
        /// </summary>
        /// <param name="driver">Hetkel kasutatav draiver (peab olema Index lehel)</param>
        /// <param name="firstName">Otsitav eesnimi</param>
        /// <returns>Tabeli rida või null, kui sellist õpilast tabelis ei ole</returns>
        private static IWebElement? FindStudentRow(IWebDriver driver, string firstName)
        {
            var rows = driver.FindElement(By.Id("IndexTable")).FindElements(By.CssSelector("tbody tr"));
            foreach (var row in rows)
            {
                if (row.FindElement(By.Id("IndexTableStudentFirstName")).Text == firstName)
                {
                    return row;
                }
            }
            return null;
        }

        /// <summary>
        /// Loeb kokku, mitu õpilast on Index lehe tabelis.
        /// </summary>
        private static int CountStudentRows(IWebDriver driver)
        {
            return driver.FindElement(By.Id("IndexTable")).FindElements(By.CssSelector("tbody tr")).Count;
        }

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
