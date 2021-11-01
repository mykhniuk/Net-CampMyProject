using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FilmsParser.Extensions;
using Net_CampMyProject.Data.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace FilmsParser
{
    class Program
    {
        private static string apiBaseUrl = "https://localhost:44303/api/";        
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //using var client = new HttpClient
            //{
            //    BaseAddress = new Uri(apiBaseUrl)
            //};

            using var driver = new ChromeDriver();

            for (int i = 2015; i <= DateTime.Now.Year; i++)
            {
                var cultureInfo = new CultureInfo("uk-UA");
                var searchString = $"Фільми {i}";

                driver.Navigate().GoToUrl("https://www.google.com/?&hl=uk");

                var acceptButton = driver.FindElements(By.XPath("//div[text() = 'Прийняти']")).FirstOrDefault();
                if(acceptButton != null)
                    acceptButton.Click();

                var serchField = driver.FindElements(By.XPath("//input[@title = 'Пошук']")).FirstOrDefault();
                if (serchField == null)
                    continue;

                serchField.SendKeys(searchString);
                serchField.SendKeys(Keys.Enter);

                var firstFilmDiv = driver.FindElements(By.XPath("//div[@jscontroller= 'eAZCyd']")).FirstOrDefault();
                if (firstFilmDiv == null)
                    continue;

                firstFilmDiv.Click();

                await Task.Delay(1000);

                firstFilmDiv.Click();

                Film newFilm = new Film()
                {

                    Title = driver.FindElements(By.XPath("//div[@class= 'SPZz6b']//span")).FirstOrDefault().Text,
                    Genre = driver.FindElements(By.XPath("//div[@data-attrid = 'subtitle']")).FirstOrDefault().Text.Split("‧").ElementAt(1).Replace(" ",""),
                    Duration = driver.FindElements(By.XPath("//div[@data-attrid = 'subtitle']")).FirstOrDefault().Text.Split("‧").ElementAt(2).Replace(" ", ""),
                    Description = driver.FindElements(By.XPath("//div[@data-attrid = 'description']//span[text()]")).FirstOrDefault().Text,
                    TrailerUrl = driver.FindElements(By.XPath("//a[@data-attrid= 'title_link']")).FirstOrDefault().GetAttribute("href").ToString(),
                    ReleaseDate = DateTime.Parse(driver.FindElements(By.XPath("//div[@data-attrid = 'kc:/film/film:theatrical region aware release date']//span[@class='Eq0J8 LrzXr kno-fv']")).FirstOrDefault().Text.Split(".").FirstOrDefault(), cultureInfo),
                    BoxOffice = driver.FindElements(By.XPath("//div[@data-attrid = 'hw:/collection/films:box office']")).FirstOrDefault().Text,





                };

            }

            

            


            //var filmRatingSources = await client.GetAsAsync<List<FilmRatingSource>>(ApiEndpoints.FilmRatingSources);
            //var filmRatingSources = await client.GetAsAsync<List<FilmPerson>>(ApiEndpoints.FilmRatingSources);
            //var filmRatingSources = await client.GetAsAsync<List<FilmPerson>>(ApiEndpoints.FilmRatingSources);


        }
    }
}
