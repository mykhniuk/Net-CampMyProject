using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Net_CampMyProject.Data.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace FilmsParser.Extensions
{
    public static class WebDriverExtensions
    {
        public static string GetTrailerUrl(this IWebDriver driver)
        {
            return driver.FindElements(By.XPath("//a[@data-attrid= 'title_link']"))
                .FirstOrDefault()?
                .GetAttribute("href")
                .Replace("watch?v=", "embed/");
        }

        public static string GetBudget(this IWebDriver driver)
        {
            return driver
                .FindElements(By.XPath("//div[@data-attrid = 'kc:/film/film:budget']"))
                .FirstOrDefault()?.Text;
        }

        public static string GetAwards(this IWebDriver driver)
        {
            return driver
                .FindElements(By.XPath("//div[@data-attrid = 'kc:/award/award_winner:awards']"))
                .FirstOrDefault()?
                .Text.Replace("Нагороди:", "").Replace(", БІЛЬШЕ","");
        }

        public static string GetDurationOrDefault(this IWebDriver driver)
        {
            var findDuration = driver.FindElements(By.XPath("//div[@data-attrid = 'subtitle']//span")).FirstOrDefault();
            if (findDuration != null) return findDuration.Text.Split("‧").LastOrDefault();

            return null;
        }

        public static string GetNominations(this IWebDriver driver)
        {
            return driver
                .FindElements(By.XPath("//div[@data-attrid = 'kc:/award/award_winner:nominations']")).FirstOrDefault()
                ?.Text.Replace("Номінації:", "").Replace(", БІЛЬШЕ","");
        }

        public static string GetBoxOffice(this IWebDriver driver)
        {
            return driver
                .FindElements(By.XPath("//div[@data-attrid = 'hw:/collection/films:box office']")).FirstOrDefault()
                ?.Text.Replace("Квиткова каса:", "");
        }


        public static string GetDescription(this IWebDriver driver)
        {
            return driver.FindElements(By.XPath("//div[@data-attrid='description']"))
                .FirstOrDefault()?.Text
                .Replace("Опис", "")
                .Replace("Вікіпедія", "").Replace("Переклад з англійської-", "").Replace("(англійська мова) Переглянути оригінальний опис", "").Replace(", БІЛЬШЕ", "")
                .Trim();
        }
        public static string GetWikiUrl(this IWebDriver driver)
        {
            return driver.FindElements(By.XPath("//div[@data-attrid='description']//div//div//span//a"))
                .FirstOrDefault().GetAttribute("href");
        }

        public static DateTime GetReleaseDateOrDefault(this IWebDriver driver, CultureInfo culture)
        {
            var dateTimeValidation = driver
                .FindElements(By.XPath(
                    "//div[@data-attrid='kc:/film/film:theatrical region aware release date']//span[@class='LrzXr kno-fv wHYlTd z8gr9e']"))
                .FirstOrDefault();

            string dtStr;

            if (dateTimeValidation == null)
                dtStr = driver
                    .FindElements(By.XPath(
                        "//div[@data-attrid='kc:/film/film:release date']//span[@class='LrzXr kno-fv wHYlTd z8gr9e']"))
                    .FirstOrDefault()?.Text.Split(".").FirstOrDefault();
            else
                dtStr = dateTimeValidation.Text.Split(".").FirstOrDefault();

            if (DateTime.TryParse(dtStr, culture, DateTimeStyles.None, out var dt)) return dt;

            return DateTime.MinValue;
        }

        public static Task TakeScreenshotOfElementAsync(this IWebDriver driver, IWebElement webElement, string filePath)
        {
            return Task.Run(() =>
            {
                var byteArray = ((ITakesScreenshot) driver).GetScreenshot().AsByteArray;

                using var memoryStream = new MemoryStream(byteArray);
                using var screenshot = new Bitmap(memoryStream);
                var croppedImage = new Rectangle(webElement.Location.X, webElement.Location.Y, webElement.Size.Width,
                    webElement.Size.Height);
                using var screenshotCloned = screenshot.Clone(croppedImage, screenshot.PixelFormat);
                screenshotCloned.Save(filePath, ImageFormat.Jpeg);
            });
        }

        public static void OpenUrlInNewTab(this WebDriver driver, string url)
        {
            driver.ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles.Last());
            driver.Navigate().GoToUrl(url);
        }

        public static async Task<Person> ParsePersonInfo(this IWebDriver driver, string imagesFolderPath)
        {
            var newPerson = new Person
            {
                Description = driver.FindElements(By.XPath("//div[@data-attrid='description']")).FirstOrDefault()?.Text
                    .Replace("Опис", "").Replace("Вікіпедія", "").Replace("(англійська мова) Переглянути оригінальний опис",""),

                BirthnIformation = driver.FindElements(By.XPath("//div[@data-attrid='kc:/people/person:born']"))
                    .FirstOrDefault()?.Text.Replace("Народження:", ""),

                Name = driver.FindElements(By.XPath("//h2[@data-attrid='title']")).FirstOrDefault()?.Text
            };

            var socialNetworksLinksChecked =
                driver.FindElements(By.XPath("//div[@data-attrid='kc:/common/topic:social media presence']//a"));
            if (socialNetworksLinksChecked.FirstOrDefault() != null)
                newPerson.SocialNetworksLinks =
                    string.Join(",", socialNetworksLinksChecked.Select(sn => sn.GetAttribute("href")));

            var imgEl = driver.FindElements(By.XPath("//g-img//img[@height ='186']")).FirstOrDefault() ??
                        driver.FindElements(By.XPath("//g-img[@data-attrid]//img")).FirstOrDefault();

            if (imgEl != null)
            {
                newPerson.ImgUrl = "/img/" + newPerson.Name + ".jpg";

                var imgPath = Path.Combine(imagesFolderPath, $"{newPerson.Name}.jpg");
                if (!File.Exists(imgPath)) await driver.TakeScreenshotOfElementAsync(imgEl, imgPath);
            }

            return newPerson;
        }

        private static string FilmPersonRoleToXpathValue(FilmPersonRole role)
        {
            switch (role)
            {
                case FilmPersonRole.Director:
                case FilmPersonRole.Screenplay:
                case FilmPersonRole.Cast:
                case FilmPersonRole.Producer:
                    return role.ToString().ToLowerInvariant();
                case FilmPersonRole.StoryBy:
                    return "story";
                case FilmPersonRole.ExecutiveProducers:
                    return "executive producer";
                case FilmPersonRole.Composer:
                    return "music";
                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, null);
            }
        }

        public static async Task<List<Person>> ParsePersonsByRoleAsync(this ChromeDriver driver, FilmPersonRole role,
            string imagesFolderPath)
        {
            var result = new List<Person>();

            switch (role)
            {
                case FilmPersonRole.Director:
                case FilmPersonRole.StoryBy:
                case FilmPersonRole.Producer:
                case FilmPersonRole.ExecutiveProducers:
                case FilmPersonRole.Composer:
                case FilmPersonRole.Screenplay:
                {
                    var personXpathBase = $"//div[@data-attrid='kc:/film/film:{FilmPersonRoleToXpathValue(role)}']";

                    var personsLabel = driver.FindElements(By.XPath(personXpathBase)).FirstOrDefault();
                    if (personsLabel == null)
                        return result;

                    var personsLinksEl = driver.FindElements(By.XPath($"{personXpathBase}//span[2]//a"));
                    if (personsLinksEl.FirstOrDefault() != null)
                    {
                        var links = personsLinksEl.Select(p => p.GetAttribute("href")).ToList();

                        foreach (var url in links)
                        {
                            driver.OpenUrlInNewTab(url);

                            if (links.Count == 1)
                            {
                                var navigate = driver.FindElements(By.XPath($"{personXpathBase}//div//a"))
                                    .FirstOrDefault()?.GetAttribute("href");
                                driver.Navigate().GoToUrl(navigate);
                            }

                            await Task.Delay(1500);

                            result.Add(await driver.ParsePersonInfo(imagesFolderPath));

                            driver.Close();
                            driver.SwitchTo().Window(driver.WindowHandles.FirstOrDefault());
                        }
                    }

                    break;
                }
                case FilmPersonRole.Cast:
                {
                    var cast = driver.FindElements(By.XPath("//div[@data-attrid = 'kc:/film/film:cast']//div//a"))
                        .FirstOrDefault();
                    if (cast == null)
                        return result;

                    var navigateToDirectorTab =
                        driver.FindElements(By.XPath("//div[@data-attrid = 'kc:/film/film:cast']//div//a"))
                            .FirstOrDefault()?.GetAttribute("href");
                    if (navigateToDirectorTab != null)
                    {
                        driver.OpenUrlInNewTab(navigateToDirectorTab);

                        var links = driver.FindElements(By.XPath("//a[@jscontroller = 'e8Ezlf']"))
                            .Select(p => p.GetAttribute("href")).ToList();

                        foreach (var url in links.Take(3))
                        {
                            driver.Navigate().GoToUrl(url);

                            if (links.Count == 1)
                            {
                                var navigate = driver
                                    .FindElements(By.XPath("//div[@data-attrid='kc:/film/film:music']//div//a"))
                                    .FirstOrDefault().GetAttribute("href");
                                driver.Navigate().GoToUrl(navigate);
                            }

                            await Task.Delay(1500);

                            result.Add(await driver.ParsePersonInfo(imagesFolderPath));
                        }

                        driver.Close();
                        driver.SwitchTo().Window(driver.WindowHandles.FirstOrDefault());
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, null);
            }

            return result;
        }
        public static string GetCurrentUrl(this IWebDriver driver)
        {
            return driver.Url;
        }
        public static List<FilmRatingRatingSourceRelated> ParsFilmRating(this ChromeDriver driver)
        {
            var result = new List<FilmRatingRatingSourceRelated>();
            var currentUrl = driver.GetCurrentUrl();
            var urlForGetRating = currentUrl.Replace("=uk", "=en");
            driver.OpenUrlInNewTab(urlForGetRating);
            driver.Navigate().GoToUrl(urlForGetRating);
            var ratingsLinks = driver.FindElements(By.XPath("//div[@data-attrid = 'kc:/film/film:reviews']//a"));
            
            if (ratingsLinks!=null)
            {
                foreach (var element in ratingsLinks)
                {
                    var newRatingSource = new FilmRatingRatingSourceRelated();
                    var newRatingSource1 = new FilmRatingSource();
                    newRatingSource1.ResourceWebsite = element.GetAttribute("href");
                    newRatingSource.RatingValue = element.Text;
                    newRatingSource.filmRatingSource = newRatingSource1;
                    result.Add(newRatingSource);
                }
                driver.Close();
                driver.SwitchTo().Window(driver.WindowHandles.FirstOrDefault());
            }
            var googleRatingsLink = driver.FindElements(By.XPath("//div[@data-attrid='kc:/ugc:thumbs_up']//div[@class='srBp4 Vrkhme']")).FirstOrDefault();
            if (googleRatingsLink !=null)
            {
                var newRatingSource = new FilmRatingRatingSourceRelated();
                var newRatingSource1 = new FilmRatingSource();
                newRatingSource1.ResourceWebsite = "https://www.google.com/";
                newRatingSource.RatingValue = googleRatingsLink.Text;
                newRatingSource.filmRatingSource = newRatingSource1;
                result.Add(newRatingSource);
            }
            return result;
        }

    }
}