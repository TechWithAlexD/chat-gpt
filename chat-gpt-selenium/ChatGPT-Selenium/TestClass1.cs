using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using SeleniumTests;

namespace ChatGPT_Selenium
{
    public class TestClass1 : TestBase
    {
        [TestCaseSource("GetTestData")]
        public void TestMethod1(TestData testData)
        {
            // Navigate to the Google homepage
            Driver.Value.Navigate().GoToUrl("https://www.selenium.dev/selenium/web/web-form.html");

            // Find the text input
            IWebElement searchBox = Driver.Value.FindElement(By.Name("my-text"));

            // Enter the search query and submit the form
            searchBox.SendKeys(testData.SearchQuery);
            searchBox.Submit();

            // Verify that the search results page is displayed
            Assert.IsTrue(Driver.Value.Title.Contains(testData.ExpectedTitle));
        }

        private static IEnumerable<TestCaseData> GetTestData()
        {
            // Read the JSON file
            string json = File.ReadAllText("testdata.json");

            // Deserialize the JSON data into a list of TestData objects
            return JsonConvert.DeserializeObject<List<TestData>>(json).Select(td => new TestCaseData(td));
        }
    }
}