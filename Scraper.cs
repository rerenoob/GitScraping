using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace GitScraping
{
    public class Scraper
    {
        const string BaseUrl = "https://github.com/search";
        const int NumPageToCrawl = 10;
        public string UrlEncodedQueryString { get; set; }
        public ChromeDriver Driver { get; set; }
        public string Url { get; set; }
        public string FileName { get; set; }
        public string Query { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<string> LinksToCrawl { get; set; }
        public List<string> Keywords { get; set; }
        public Scraper(ChromeDriver driver, string query, string fileName, string username, string password)
        {
            Driver = driver;
            UrlEncodedQueryString = HttpUtility.UrlEncode(query) + "&type=Code";
            Url = BaseUrl + "?q=" + UrlEncodedQueryString;
            Username = username;
            Password = password;
            FileName = fileName;
            Query = query;
            LinksToCrawl = new List<string>();
            Keywords = new List<string>();
            Keywords.Add("host");
            Keywords.Add("user");
            Keywords.Add("username");
            Keywords.Add("pass");
            Keywords.Add("password");
            Keywords.Add("port");
        }

        public void Login()
        {
            Driver.Navigate().GoToUrl(Url);
            var usernameFieldSelector = By.XPath("//*[@id=\"login_field\"]");
            var passwordFieldSelector = By.XPath("//*[@id=\"password\"]");
            var submitButtonSelector = By.XPath("//*[@id=\"login\"]/form/div[3]/input[7]");
            var usernameField = Driver.FindElement(usernameFieldSelector);
            var passwordField = Driver.FindElement(passwordFieldSelector);
            var submitButton = Driver.FindElement(submitButtonSelector);
            usernameField.SendKeys(Username);
            passwordField.SendKeys(Password);
            submitButton.Click();
            
            Thread.Sleep(60000); // wait 15 second for the verification code
        }

        public void GetLinksToCrawl()
        {
            for (int i = 1; i < NumPageToCrawl;i++){
                var url = BaseUrl + "?p=" + i + "&q=" + UrlEncodedQueryString;
                Driver.Navigate().GoToUrl(url);
                var allLinks = Driver.FindElementsByTagName("a");

                foreach (var link in allLinks)
                {
                    if (link.Text.Contains(FileName))
                    {
                        var linkHref = link.GetAttribute("href");
                        if (!LinksToCrawl.Contains(linkHref))
                            LinksToCrawl.Add(linkHref);
                    }
                }
            }
        }

        public void ScrapeLinks()
        {
            string currentPath = Directory.GetCurrentDirectory();
            string filePath = Path.Combine(currentPath, FileName);
            foreach (var link in LinksToCrawl)
            {
                using (StreamWriter outputFile = new StreamWriter(filePath, true))
                {
                    try{
                        Driver.Navigate().GoToUrl(link);
                        outputFile.WriteLine("Scraping " + link);
                        var allCodeLine = Driver.FindElementsByTagName("td");
                        foreach (var line in allCodeLine)
                        {
                            if(Keywords.Any(key => line.Text.ToLower().Contains(key))){
                                outputFile.WriteLine(line.Text);
                            }
                        }
                    }catch(Exception ex){
                        outputFile.WriteLine("Unable to scrape: " + link);
                    }
                }
            }
        }

        public void Dispose(){
            Driver.Dispose();
        }
    }
}