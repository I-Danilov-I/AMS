using OpenQA.Selenium;

namespace AMST
{
    internal class WebWaitForElementHelper
    {
        internal static IWebElement? WaitHelper(IWebDriver driver, By locator, string elementName, string? sendTextToField = null)
        {
            int searchRate = 1000;
            int attempts = 0;

            while (attempts < 12)
            {
                try
                {
                    attempts++;
                    Thread.Sleep(searchRate);
                    var element = driver.FindElement(locator);

                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({ behavior: 'smooth', block: 'center' });", element);
                    Thread.Sleep(searchRate);
                    if (sendTextToField != null && (element.TagName == "input" || element.TagName == "textarea" || element.TagName == "select" && element.GetAttribute("type") == "text"))
                    {
                        string existingText = element.GetAttribute("value") ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(sendTextToField))
                        {
                            return element;
                        }
                        element.Clear();
                        element.SendKeys(sendTextToField);
                    }
                    else
                    {
                        element.Click();
                    }
                    return element;
                }
                catch{}
            }

            Thread.Sleep(3000);
            return null;
        }
    }
}
