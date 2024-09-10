using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Newtonsoft.Json;
using System.Reflection;

namespace AMST
{

    internal static class TaleoOperations
    {
        private static string? _firstName;
        private static string? _lastName;
        private static string? _birthday;
        private static string? _message;
        private static string? _requisitionID;
        internal static string? _email;
        private static string? _street;
        private static string? _houseNumber;
        private static string? _city;
        private static string? _zipCode;
        private static string? _phone;
        private static string? _salutation;
        private static string? _sourceType;
        private static string? _source;
        private static string? _socialMedia;
        private static string? _language;
        private static IWebElement? _checkDuplicate;


        internal static void GetDataForTaleo()
        {
            try
            {
                Logger.Log.Information("Get data for Taleo...");
                string? ProgramDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (ProgramDirectory == null) { Logger.Log.Information("Program directory not find."); throw new Exception(); };

                var jsonFiles = Directory.GetFiles(Path.Combine(ProgramDirectory, "Code\\JsonOperations\\TempJsonFiles\\").Replace("bin\\Debug\\net8.0", ""), "*.json");

                Array.Sort(jsonFiles);
                string jsonPath = jsonFiles[0];
                var fileContent = File.ReadAllText(jsonPath);
                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContent);
                if (values != null)
                {
                    _firstName = values["FirstName"];
                    _lastName = values["LastName"];
                    _birthday = values["BirthDate"];
                    _message = values["Message"];
                    _requisitionID = values["RequisitionID"];
                    _email = "anatoliy.danilov@hellmann.com"; // values["Email"]?.ToLower();
                    _street = values["Street"];
                    _houseNumber = values["HouseNumber"];
                    _city = values["City"];
                    _zipCode = values["ZipCode"];
                    _phone = values["Phone"];
                    _salutation = values["Salutation"];
                    _socialMedia = values["SocialMedia"];
                    _sourceType = values["SourceType"];
                    _source = values["Source"];
                    _language = values["Language"];

                    Logger.Log.Information("Get data for Taleo succesfully");
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Get data for Taleo: {ex.Message}");
                throw;
            }
        }


        internal static void LoginToTaleo(EdgeDriver driver)
        {
            try
            {
                Logger.Log.Information("Login...");
                driver.Navigate().GoToUrl(LoadLogDataForTaleo.GetTaleoStartsite());

                _ = WebWaitForElementHelper.WaitHelper(driver, By.Id("dialogTemplate-dialogForm-content-login-name1"), "Benutzername Login", LoadLogDataForTaleo.GetTaleoLoginUsername()) ?? throw new Exception("Benutzername Login Feld nicht gefunden.");
                IWebElement password = WebWaitForElementHelper.WaitHelper(driver, By.Id("dialogTemplate-dialogForm-content-login-password"), "Passwort", LoadLogDataForTaleo.GetTaleoLoginPassword()) ?? throw new Exception("Passwort Feld nicht gefunden.");
                password?.SendKeys(Keys.Enter);

                // Überprüfung nach einem Error Dialog nach der Eingabe von Login.
                try
                {
                    Thread.Sleep(3000);
                    IWebElement? errorDialog = driver.FindElement(By.Id("dialogTemplate-dialogForm-content-login-errorMessageTitle"));
                    if (errorDialog.Displayed)
                    {
                        IWebElement? errorDialogUnder = driver.FindElement(By.Id("dialogTemplate-dialogForm-content-login-errorMessageContent"));
                        Logger.Log.Fatal($"Errordialog: {errorDialog.Text}\n{errorDialogUnder.Text}");
                        Environment.Exit(0);
                    }
                }
                catch (NoSuchElementException)
                {
                    Logger.Log.Information("Login succesfully!");
                }

            }
            catch(Exception ex) 
            {
                Logger.Log.Error($"Login: {ex.Message}");
                throw;
            }
        }


        internal static void NavigationToCandidate(EdgeDriver driver)
        {
            try
            {
                Logger.Log.Information("Navigate to candidate...");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.Id("menuTemplate-menuForm-globalHeader-pageRibbonSubView-j_id_id29pc12-0-ribbonItemText"), "Recurting") ?? throw new Exception("Recruiting button not found.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//span[@class='oj-navigationlist-item-label']/span[text()='Candidates']"), "Candidates") ?? throw new Exception("Candidates button not found.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.ClassName("actionbar__more"), "More Actions") ?? throw new Exception("More Actions button not found.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.Id("createCandidate"), "Create Candidate") ?? throw new Exception("Create Candidate button not found.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//span[text()='Next']"), "Next Button") ?? throw new Exception("Next button not found.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.Id("ownershipFilter"), "Filters") ?? throw new Exception("Filters field not found.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//div[@aria-label='All requisitions']"), "All Requisitions") ?? throw new Exception("All Requisitions button not found.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.Id("contestNumber"), "Requisition ID", _requisitionID) ?? throw new Exception("Requisition ID field not found.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[text()='Apply Filters']"), "Apply Filter") ?? throw new Exception("Apply Filters button not found.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath($"//td[text()='{_requisitionID}']"), "Applicant Requisition ID") ?? throw new Exception("Applicant Requisition ID not found.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//span[text()='Next']"), "Next Button") ?? throw new Exception("Second Next button not found.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//span[text()='Next']"), "Next Button") ?? throw new Exception("Third Next button not found.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//span[text()='Skip']"), "Skip Button") ?? throw new Exception("Skip button not found.");
                Logger.Log.Information("Navigate to candidate successfully!");
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Navigate to candidate: {ex.Message}");
                throw;
            }
        }


        internal static bool CheckDuplicateAndCanModifyCandidate(IWebDriver driver)
        {
            _ = WebWaitForElementHelper.WaitHelper(driver, By.Id("email-value|input"), "Suche nach dupliziertem Kandidaten per E-Mail", _email) ?? throw new Exception("Eingabefeld für die Suche nach dupliziertem Kandidaten per E-Mail nicht gefunden.");
            _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//span[text()='Next']"), "Weiter-Schaltfläche") ?? throw new Exception("Weiter-Schaltfläche nicht gefunden.");

            _checkDuplicate = WebWaitForElementHelper.WaitHelper(driver, By.XPath($"//td[contains(text(), '{_email}')]"), "Suche nach E-Mail-Übereinstimmung");
            if (_checkDuplicate != null)
            {
                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//span[text()='Next']"), "Weiter-Schaltfläche") ?? throw new Exception("Weiter-Schaltfläche nicht gefunden.");
                Thread.Sleep(4000);
                try
                {
                    IWebElement? warningDialog = driver.FindElement(By.Id("errorDialog"));
                    if (warningDialog.Displayed)
                    {
                        IWebElement? warningMessageElement = driver.FindElement(By.CssSelector("#navigation-warning-content p"));
                        string warningText = warningMessageElement.Text;
                        Logger.Log.Warning($"Navigate to candidate: {warningText}");
                        return false;
                    }
                }
                catch (NoSuchElementException) { }
                return true;
            }
            else
            {
                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//span[text()='Next']"), "Weiter-Schaltfläche") ?? throw new Exception("Weiter-Schaltfläche nicht gefunden.");
                return true;
            }
        }


        internal static bool SelectFromDropMenu(IWebDriver driver, string elementIdOpen, string elementIdResult, string searchItem)
        {
            try
            {
                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
                var dropdownToggle = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id(elementIdOpen)));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({ behavior: 'smooth', block: 'center' });", dropdownToggle);
                dropdownToggle.Click();
                string optionsListXPath = $"//ul[@id='{elementIdResult}']/li/div[@role='option']";
                wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(optionsListXPath)));
                IList<IWebElement> options = driver.FindElements(By.XPath(optionsListXPath));
                foreach (var option in options)
                {
                    string item = option.Text.Trim().Replace(" ", "");
                    if (string.IsNullOrEmpty(item))
                    {
                        item = option.GetAttribute("textContent").Trim().Replace(" ", "");
                    }

                    if (item.Equals(searchItem.Replace(" ", ""), StringComparison.CurrentCultureIgnoreCase))
                    {
                        Thread.Sleep(1000);
                        option.Click();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
            return false;
        }


        internal static bool InputCandidateData(IWebDriver driver)
        {
            try
            {
                Logger.Log.Information("Input candidate data...");
                if (_salutation != null)
                {
                    bool dropdownReturn = SelectFromDropMenu(driver, "10883_profile_profileInformation_candidate_HWL_5fSALUTATION-value", "oj-listbox-results-10883_profile_profileInformation_candidate_HWL_5fSALUTATION-value", _salutation);
                    if (!dropdownReturn)
                    {
                        SelectFromDropMenu(driver, "oj-select-choice-10859_application_profileInformation_candidate_HWL_5fSALUTATION-value", "oj-listbox-results-10859_application_profileInformation_candidate_HWL_5fSALUTATION-value", _salutation);
                    }
                }

                _ = WebWaitForElementHelper.WaitHelper(driver, By.Id("firstName-value|input"), "Vorname", _firstName) ?? throw new Exception("Vorname-Feld nicht gefunden.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.Id("lastName-value|input"), "Nachname", _lastName) ?? throw new Exception("Nachname-Feld nicht gefunden.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.Id("address-value|input"), "Straße", _street) ?? throw new Exception("Straßen-Feld nicht gefunden.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.Id("city-value|input"), "Stadt", _city) ?? throw new Exception("Stadt-Feld nicht gefunden.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.Id("address2-value|input"), "Hausnummer", _houseNumber) ?? throw new Exception("Hausnummer-Feld nicht gefunden.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.Id("birthday-value|input"), "Geburtsdatum", _birthday) ?? throw new Exception("Geburtsdatum-Feld nicht gefunden.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.Id("zipCode-value|input"), "PLZ", _zipCode) ?? throw new Exception("PLZ-Feld nicht gefunden.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.Id("mobilePhone-value|input"), "Telefon", _phone) ?? throw new Exception("Telefon-Feld nicht gefunden.");

                IWebElement? personalMessage = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//input[@id='10883_profile_profileInformation_candidate_HWL_5fPersonal_5fMessage-value|input']"), "Persönliche Nachricht", _message);
                if (personalMessage == null)
                {
                    _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[@id='10859_application_profileInformation_candidate_HWL_5fPersonal_5fMessage-value|input']"), "Persönliche Nachricht 2", _message) ?? throw new Exception("Persönliche Nachricht nicht gefunden.");
                }

                _ = WebWaitForElementHelper.WaitHelper(driver, By.Id("emailAddress-value|input"), "E-Mail-Adresse", _email) ?? throw new Exception("E-Mail-Feld nicht gefunden.");
                IWebElement? socialMediaLink = WebWaitForElementHelper.WaitHelper(driver, By.Id("10883_profile_profileInformation_candidate_HWL_5fSocial_5fMedia_5fLink-value"), "Social-Media-Link 1", _socialMedia);
                if (socialMediaLink == null)
                {
                    _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[@id='10859_application_profileInformation_candidate_HWL_5fSocial_5fMedia_5fLink-value|input']"), "Social-Media-Link 2", _socialMedia) ?? throw new Exception("Social-Media-Link 2 Feld nicht gefunden.");
                }

                if (_sourceType != null)
                {
                    SelectFromDropMenu(driver, "oj-select-choice-sourceType", "oj-listbox-results-sourceType", _sourceType);
                }
                if (_source != null)
                {
                    SelectFromDropMenu(driver, "oj-select-choice-sourceTypeDetail", "oj-listbox-results-sourceTypeDetail", _source);
                }

                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[text()='Done']"), "Done-Schaltfläche") ?? throw new Exception("Done-Schaltfläche nicht gefunden.");
                Logger.Log.Information("Input candidate data succesfully!");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Input candidate data: {ex.Message}");
                return false;
            }
        }


        internal static void CheckSuccessMessage(IWebDriver driver)
        {
            try
            {
                Logger.Log.Information("Check if Candidate data save...");
                IWebElement? successMessage = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//div[@class='message-box__content' and contains(text(), 'Candidate Saved Successfully')]"), "Nachricht Successfully saved");
                Logger.Log.Information("Candidate data save succesfully!");
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Candidate data in Taleonot save: {ex.Message}");
                throw;
            }
        }

        internal static void UploadAttachments(IWebDriver driver, string[] allowedExtensions)
        {
            Logger.Log.Information("Upload Attachments to Taleo...");
            string? ProgramDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (ProgramDirectory == null) { Logger.Log.Information("Program directory not find."); throw new Exception(); };

            string path = Path.Combine(ProgramDirectory, "Code\\MailOperations\\TempAttachmentsFiles\\").Replace("\\bin\\Debug\\net8.0", "");
            try
            {
                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[@id='app-attachment-tab']"), "App-attachment-tab") ?? throw new Exception("App-attachment-tab not found.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[@id='app-attachment-tab']"), "Again App-attachment-tab") ?? throw new Exception("Again App-attachment-tab not found.");

                var allFiles = allowedExtensions.SelectMany(ext => Directory.GetFiles(path, $"*{ext}")).ToArray();

                if (allFiles.Length == 0)
                {
                    throw new Exception("No valid Attachments format.");
                }
                Array.Sort(allFiles);

                foreach (var file in allFiles)
                {
                    if (_checkDuplicate == null)
                    {
                        _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[text()='Upload Attachment']"), "Upload first Attachment");
                    }
                    else
                    {
                        _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[text()='Upload Other Attachment']"), "Upload Other Attachment") ?? throw new Exception("Upload Other Attachment not found.");
                    }

                    string fileName = Path.GetFileName(file);
                    string fullPath = Path.Combine(path, fileName);
                    driver.SwitchTo().Window(driver.WindowHandles.Last());
                    IWebElement uploadElement = driver.FindElement(By.CssSelector("input[type='file']"));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].style.display = 'block';", uploadElement);
                    uploadElement.SendKeys(fullPath);

                    _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[text()='Upload']"), "Upload");

                    CheckWarning(driver, fileName);

                    IWebElement checkUpload = driver.FindElement(By.XPath($"//*[text()='{fileName}']"));
                    if (checkUpload.Text == fileName)
                    {
                        Logger.Log.Information($"Upload Successfully: '{Path.GetFileName(file)}'");
                    }
                    else
                    {
                        Logger.Log.Error($"Upload failed: {Path.GetFileName(file)}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Upload Attachments to Taleo: {ex.Message}");
                throw;
            }
        }

        internal static void CheckWarning(IWebDriver driver, string fileName)
        {
            Thread.Sleep(4000);
            driver.FindElement(By.Id("errorDialog"));

            try
            {
                _ = WebWaitForElementHelper.WaitHelper(driver, By.Id("yesBtn"), "Button: Yes") ?? throw new Exception("Yes Button not found.");
            }
            catch { }

            try
            {
                Thread.Sleep(4000);
                IWebElement? errorContainer = driver.FindElement(By.CssSelector(".attachments-popup__error-container"));
                if (errorContainer != null && errorContainer.Displayed)
                {
                    var errorMessageElement = errorContainer.FindElement(By.CssSelector(".message-box__content span"));
                    string errorMessage = errorMessageElement.Text;
                    Logger.Log.Information($"{fileName} \n{errorMessage}");
                    throw new Exception($"Upload error: {fileName} \n{errorMessage}");
                }
            }
            catch (NoSuchElementException) { }
        }

        internal static bool SendEmailReply(IWebDriver driver)
        {
            try
            {
                Logger.Log.Information("Send Confirmation Email...");
                driver.SwitchTo().Window(driver.WindowHandles.First());
                Thread.Sleep(1000);

                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[text()='More Actions']"), "More Actions") ?? throw new Exception("More Actions Button not found.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[@id='ACTION_6']"), "Send Correspondence") ?? throw new Exception("Send Correspondence Button not found.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[text()='Next']"), "Next") ?? throw new Exception("Next Button not found.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[@id='filtersLabel']"), "Filter") ?? throw new Exception("Filter Button not found.");

                if (_language == "de")
                {
                    _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[@id='keywordinput']"), "SearchItem", "RPA Bot Application DE") ?? throw new Exception("SearchItem not found.");
                }
                if (_language == "en")
                {
                    _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[@id='keywordinput']"), "SearchItem", "RPA Bot Application EN") ?? throw new Exception("SearchItem not found.");
                }

                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[text()='Apply Filters']"), "FilterApply") ?? throw new Exception("FilterApply Button not found.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[text()='Select Template']"), "Select Template") ?? throw new Exception("Select Template Button not found.");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[text()='Send']"), "Send") ?? throw new Exception("Send Button not found.");

                IWebElement? successMessage = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//span[contains(text(), 'Successfully sent')]"), "Check if email was sent successfully") ?? throw new Exception("Message sending failed.");
                if (successMessage.Displayed)
                {
                    _ = WebWaitForElementHelper.WaitHelper(driver, By.XPath("//*[@id='actionResultClose']"), "Close") ?? throw new Exception("Close Button not found.");
                    Logger.Log.Information("Confirmation email sent successfully!");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Confirmation email could not be sent: {ex.Message}");
                throw;
            }
        }

        internal static void Logout(IWebDriver driver)
        {
            try
            {
                Logger.Log.Information("Logout...");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.Id("mainmenu-user"), "Mainmenu-User");
                _ = WebWaitForElementHelper.WaitHelper(driver, By.Id("mainmenu-user-logout"), "Logout");
                IWebElement? checkFinish = WebWaitForElementHelper.WaitHelper(driver, By.Id("dialogTemplate-dialogForm-content-pageHeaderSubView-pageHeaderSubView-pageHeaderTitleText"), "Check Successfully");
                if (checkFinish != null)
                {
                    Logger.Log.Information($"{checkFinish.Text}");
                    Thread.Sleep(3000);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Warning($"Logout failed: {ex.Message}");
            }
        }
    }
}
