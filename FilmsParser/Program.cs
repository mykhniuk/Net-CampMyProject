using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FilmsParser.Extensions;
using Microsoft.EntityFrameworkCore.Internal;
using Net_CampMyProject.Data.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace FilmsParser
{
    class Program
    {
        private static readonly string SearchStringPrefix = "Фільми";
        private static readonly string AcceptKeyWord = "Прийняти";
        private static readonly string SearchKeyword = "Пошук";
        private static readonly int StartFromYear = 2020;

        private static readonly CultureInfo CultureInfo = new("uk-UA");
        private static readonly string imagesFolderPath = @"E:\New folder (3)\Net CampMyProject\wwwroot\img\";

        private static string apiBaseUrl = "https://watchmovie.today/api/";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            using var apiClient = new ApiClient(apiBaseUrl);
            using var driver = new ChromeDriver();

            for (var i = StartFromYear; i <= DateTime.Now.Year; i++)
            {
                driver.Navigate().GoToUrl("https://www.google.com/?&hl=uk");
                driver.FindElements(By.XPath($"//div[text() = '{AcceptKeyWord}']")).FirstOrDefault()?.Click();
                var searchField = driver.FindElements(By.XPath($"//input[@title = '{SearchKeyword}']")).FirstOrDefault();
                if (searchField == null)
                    continue;

                searchField.SendKeys($"{SearchStringPrefix} {i}");
                searchField.SendKeys(Keys.Enter);
                
                var firstFilm = driver.FindElement(By.XPath("//div[@data-index='0']//div[@data-index='0']"));
                firstFilm.Click();
                await Task.Delay(1000);
                firstFilm.Click();

                var filmInfoList = driver.FindElements(By.XPath("//a[@jscontroller='e8Ezlf']"));

                foreach (var filmInfoEl in filmInfoList)
                {
                  filmInfoEl.Click();
                    var filmTitle = filmInfoEl.GetAttribute("aria-label");
                    
                    if (await apiClient.GetFilmByTitleOrDefaultAsync(filmTitle) != null)
                    {
                       filmInfoEl.Click();
                       await Task.Delay(2000);
                        continue;
                    }

                    

                    await Task.Delay(3000);

                    var newFilm = new Film
                    {
                        Title = filmTitle,
                        Description = driver.GetDescription(),
                        ReleaseDate = driver.GetReleaseDateOrDefault(CultureInfo),
                        TrailerUrl = driver.GetTrailerUrl(),
                        Budget = driver.GetBudget(),
                        Awards = driver.GetAwards(),
                        BoxOffice = driver.GetBoxOffice(),
                        Nominations = driver.GetNominations(),
                        Duration = driver.GetDurationOrDefault(),
                        WikiUrl = driver.GetWikiUrl()
                    };
                    await Task.Delay(3000);
                    var filmImg = filmInfoEl.FindElements(By.XPath("//g-img//img[@height ='186']")).FirstOrDefault() ??
                                  filmInfoEl.FindElements(By.XPath("//g-img[@data-attrid]//img")).FirstOrDefault();

                    if (filmImg != null)
                    {
                        newFilm.ImgUrl = "/img/" + newFilm.Title + ".jpg";

                        var imgPath = Path.Combine(imagesFolderPath, $"{newFilm.Title}.jpg");
                        await driver.TakeScreenshotOfElementAsync(filmImg, imgPath);
                    }

                    //ADD NEW FILM
                    var addedFilm = await apiClient.AddFilmAsync(newFilm);
                    if (addedFilm == null)
                        continue;
                    await Task.Delay(3000);
                    //GENRES
                    var filmYearGenresEl = driver.FindElements(By.XPath("//div[@data-attrid = 'subtitle']//span")).FirstOrDefault();
                    if (filmYearGenresEl != null)
                    {
                        var findGenreArr = filmYearGenresEl.Text.Split("‧");
                        if (findGenreArr.Length > 2)
                        {
                            foreach (var filmGenre in findGenreArr[1].Split("/").Select(g => g.Trim()))
                            {
                                var genre = await apiClient.AddOrGetGenreAsync(filmGenre);
                                await apiClient.AddFilmGenreAsync(addedFilm.Id, genre.Id);
                            }
                        }
                    }
                    await Task.Delay(3000);
                    //PERSONS
                    foreach (var role in Enum.GetValues<FilmPersonRole>())
                    {
                        foreach (var k in await driver.ParsePersonsByRoleAsync(role, imagesFolderPath))
                        {
                            var person = await apiClient.AddOrGetPersonAsync(k);
                            await apiClient.AddFilmPerson(addedFilm.Id, person.Id, role);
                        }
                    }
                    await Task.Delay(3000);
                    //RATINGS
                    foreach (var rating in driver.ParsFilmRating())
                    {
                        var ratingSource = await apiClient.AddOrGetRatingSource(rating.filmRatingSource.ResourceWebsite);
                        await apiClient.AddFilmRating(addedFilm.Id, ratingSource.Id, rating.RatingValue);
                        
                    }
                }
            }
        }

        


    }
}
