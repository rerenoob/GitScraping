using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NDesk.Options;
using OpenQA.Selenium.Chrome;

namespace GitScraping
{
    class Program
    {
        static void Main(string[] args)
        {
            string username = "", password = "", query = "", fileName = "";
            bool show_help = false;
            var p = new OptionSet() {
                { "u|username=", "{USERNAME} of your github account.",
                    v => username = v},
                { "p|password=", "{PASSWORD} of your github account.",
                    v => password = v},
                { "q|query=", "github {QUERY} to scrap.",
                    v => query = v},
                { "f|file=", "{FILE} name in the query. Scraped data is stored in this file.",
                    v => fileName = v},
                { "h|help", "show this message and exit",
                    v => show_help = v != null }
            };

            List<string> extra;
            try {
                extra = p.Parse (args);
            }
            catch (OptionException e) {
                Console.Write ("GitScraping: ");
                Console.WriteLine (e.Message);
                Console.WriteLine ("Try `GitScraping --help' for more information.");
                return;
            }

            if (show_help || 
                    string.IsNullOrEmpty(username) || 
                    string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(query) ||
                    string.IsNullOrEmpty(fileName))
                {
                ShowHelp(p);
                return;
            }
            
            var browserDriverPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            using(var driver = new ChromeDriver(browserDriverPath)){
                var scraper = new Scraper(driver, query, fileName, username, password);
                scraper.Login();  
                scraper.GetLinksToCrawl();
                scraper.ScrapeLinks();
                scraper.Dispose();
            }
        }

        private static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: GitScraping [PARAMETERS]");
            Console.WriteLine("Scrape credentials on publicly available git repos.");
            Console.WriteLine("All 4 parameters are required.");
            Console.WriteLine();
            Console.WriteLine("Parameters:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
