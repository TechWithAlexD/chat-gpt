using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace SeleniumTests
{
    public class TestBase
    {
        protected static ThreadLocal<IWebDriver> Driver = new ThreadLocal<IWebDriver>();
        protected ExtentTest test;
        protected static ExtentReports extent = new ExtentReports();

        [OneTimeSetUp]
        public void SetUp()
        {
            // Set up the ExtentReports object
            extent.AttachReporter(new ExtentHtmlReporter("TestResults"));
        }

        [SetUp]
        public void TestSetUp()
        {
            ChromeOptions options = new ChromeOptions();

            // Add any desired Chrome options here
            options.AddArgument("--start-maximized");

            Driver.Value = new ChromeDriver(options);

            // Set the implicit wait time to 10 seconds
            Driver.Value.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            // Start the Extent test
            test = extent.CreateTest(TestContext.CurrentContext.Test.Name);
        }

        [TearDown]
        public void TearDown()
        {
            // Stop the Extent test and write the result to the report
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            var stackTrace = "<pre>" + TestContext.CurrentContext.Result.StackTrace + "</pre>";
            var errorMessage = TestContext.CurrentContext.Result.Message;

            if (status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                test.Log(Status.Fail, stackTrace + errorMessage);
            }
            else
            {
                test.Log(Status.Pass, "Test passed");
            }

            // Check if the test has failed
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                // Take a screenshot if the test has failed
                Screenshot screenshot = ((ITakesScreenshot)Driver.Value).GetScreenshot();
                
                //create directory
                Directory.CreateDirectory("TestResults");

                string filePath = Path.Combine("TestResults", TestContext.CurrentContext.Test.FullName + ".png");
                screenshot.SaveAsFile(filePath, ScreenshotImageFormat.Png);

                //attach screenshot to to reporter
                test.Log(Status.Fail, MediaEntityBuilder.CreateScreenCaptureFromPath(filePath).Build());
            }

            try
            {
                Driver.Value.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // Flush the ExtentReports object and write the report to disk
            extent.Flush();
        }
    }
}