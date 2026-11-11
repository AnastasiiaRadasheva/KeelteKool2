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
