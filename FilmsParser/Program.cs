using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FilmsParser.Extensions;
using Net_CampMyProject.Data.Models;
using Newtonsoft.Json;
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

            using var client = new HttpClient
            {
                BaseAddress = new Uri(apiBaseUrl)
            };

            using var driver = new ChromeDriver();

            for (int i = 2015; i <= DateTime.Now.Year; i++)
            {
                var cultureInfo = new CultureInfo("uk-UA");
                var searchString = $"Фільми {i}";

                driver.Navigate().GoToUrl("https://www.google.com/?&hl=en");

                var acceptButton = driver.FindElements(By.XPath("//div[text() = 'Прийняти']")).FirstOrDefault();
                if (acceptButton != null)
                    acceptButton.Click();

                var serchField = driver.FindElements(By.XPath("//input[@title = 'Search']")).FirstOrDefault();
                if (serchField == null)
                    continue;

                serchField.SendKeys(searchString);
                serchField.SendKeys(Keys.Enter);

                Film newFilm = new Film();
                var firstFilm = driver.FindElement(By.XPath("//div[@data-index='0']//div[@data-index='0']"));
                firstFilm.Click();
                Thread.Sleep(1000);
                firstFilm.Click();



                var filmInfoList = driver.FindElements(By.XPath("//a[@jscontroller='e8Ezlf']"));

                foreach (var f in filmInfoList)
                {
                    newFilm.Title = f.GetAttribute("aria-label");
                    f.Click();
                    Thread.Sleep(1500);

                    var filmImg = f.FindElements(By.XPath("//g-img//img[@height ='186']")).FirstOrDefault();
                        if(filmImg != null)
                        {
                            newFilm.ImgUrl = "/img/" + newFilm.Title + ".jpg";
                            await Task.Run(() => TakeFilmScreenshot(driver, filmImg, newFilm));
                            Thread.Sleep(500);
                        }
                    
                    var filmImg1 = f.FindElements(By.XPath("//g-img[@data-attrid]//img")).FirstOrDefault();
                    if (filmImg != null)
                    {
                        newFilm.ImgUrl = "/img/" + newFilm.Title + ".jpg";
                        await Task.Run(() => TakeFilmScreenshot(driver, filmImg, newFilm));
                        Thread.Sleep(500);
                    }

                    newFilm.Description = driver.FindElements(By.XPath("//div[@data-attrid='description']")).FirstOrDefault()?
                            .Text.Replace("Опис", "").Replace("Вікіпедія", "");
                    var dateTimeValidetion = driver.FindElements(By.XPath("//div[@data-attrid = 'kc:/film/film:theatrical region aware release date']//span[@class='Eq0J8 LrzXr kno-fv']")).FirstOrDefault();
                    if (dateTimeValidetion == null)
                    {
                        try
                        {
                            newFilm.ReleaseDate = DateTime.Parse(driver.FindElements(By.XPath("//div[@data-attrid='kc:/film/film:release date']//span[@class='Eq0J8 LrzXr kno-fv']")).FirstOrDefault()?.Text.Split(".").FirstOrDefault(), cultureInfo);
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            newFilm.ReleaseDate = DateTime.Parse(dateTimeValidetion?.Text.Split(".").FirstOrDefault(), cultureInfo);
                        }
                        catch { }
                    }
                    newFilm.TrailerUrl = driver.FindElements(By.XPath("//a[@data-attrid= 'title_link']")).FirstOrDefault()?.GetAttribute("href").Replace("watch?v=", "embed/");

                    newFilm.Budget = driver.FindElements(By.XPath("//div[@data-attrid = 'kc:/film/film:budget']")).FirstOrDefault()?.Text;

                    newFilm.Awards = driver
                            .FindElements(By.XPath("//div[@data-attrid = 'kc:/award/award_winner:awards']")).FirstOrDefault()?.Text.Replace("Нагороди:", "");

                    newFilm.BoxOffice = driver
                            .FindElements(By.XPath("//div[@data-attrid = 'hw:/collection/films:box office']")).FirstOrDefault()?.Text.Replace("Квиткова каса:", "");
                    newFilm.Nominations = driver
                            .FindElements(By.XPath("//div[@data-attrid = 'kc:/award/award_winner:nominations']")).FirstOrDefault()?.Text.Replace("Номінації:", "");

                    var getAllFilms = await client.GetAsAsync<List<Film>>(ApiEndpoints.Films);
                    var addFilmValidation = getAllFilms.FirstOrDefault(f => f.Title == newFilm.Title);
                    if (addFilmValidation == null)
                    {
                        var newFilmJsonStr = JsonConvert.SerializeObject(newFilm);
                        var postRes = client.PostAsync(ApiEndpoints.Films, new StringContent(newFilmJsonStr, Encoding.UTF8, "application/json")).Result;
                    }
                    List<Film> allFilms = await client.GetAsAsync<List<Film>>(ApiEndpoints.Films);
                    var newFilmId = allFilms.FirstOrDefault(f => f.Title == newFilm.Title).Id;

                    var director = driver.FindElements(By.XPath("//div[@data-attrid='kc:/film/film:director']")).FirstOrDefault();

                    if (director != null)
                    {
                        var newPerson = PersonParsingAsync(driver, FilmPersonRole.Director).Result;

                        var allPersons = await client.GetAsAsync<List<Person>>(ApiEndpoints.Persons);
                        foreach (var k in newPerson)
                        {
                            var s = allPersons.FirstOrDefault(p => p.Name == k.Name);
                            if (s == null)
                            {
                                var newPersomJsonStr = JsonConvert.SerializeObject(k);
                                var postResPerson = client.PostAsync(ApiEndpoints.Persons, new StringContent(newPersomJsonStr, Encoding.UTF8, "application/json")).Result;

                                var allPersonsID = await client.GetAsAsync<List<Person>>(ApiEndpoints.Persons);
                                var newPersonId = allPersonsID.FirstOrDefault(f => f.Name == k.Name).Id;
                                await AddFilmPerson(client, newFilmId, newPersonId, FilmPersonRole.Director);
                            }
                        }

                    }
                    Thread.Sleep(1500);
                    var musicComposer = driver.FindElements(By.XPath("//div[@data-attrid='kc:/film/film:music']")).FirstOrDefault();
                    if (musicComposer != null)
                    {
                        var newPerson = PersonParsingAsync(driver, FilmPersonRole.Composer).Result;

                        var allPersons = await client.GetAsAsync<List<Person>>(ApiEndpoints.Persons);

                        foreach (var k in newPerson)
                        {
                            var s = allPersons.FirstOrDefault(p => p.Name == k.Name);
                            if (s == null)
                            {
                                var newPersomJsonStr = JsonConvert.SerializeObject(k);
                                var postResPerson = client.PostAsync(ApiEndpoints.Persons, new StringContent(newPersomJsonStr, Encoding.UTF8, "application/json")).Result;

                                var allPersonsID = await client.GetAsAsync<List<Person>>(ApiEndpoints.Persons);
                                var newPersonId = allPersonsID.FirstOrDefault(f => f.Name == k.Name).Id;
                                await AddFilmPerson(client, newFilmId, newPersonId, FilmPersonRole.Composer);
                            }
                        }
                    }

                    var executiveProducers = driver.FindElements(By.XPath("//div[@data-attrid='kc:/film/film:executive producer']")).FirstOrDefault();
                    if (executiveProducers != null)
                    {
                        var newPerson = PersonParsingAsync(driver, FilmPersonRole.ExecutiveProducers).Result;

                        var allPersons = await client.GetAsAsync<List<Person>>(ApiEndpoints.Persons);

                        foreach (var k in newPerson)
                        {
                            var s = allPersons.FirstOrDefault(p => p.Name == k.Name);
                            if (s == null)
                            {
                                var newPersomJsonStr = JsonConvert.SerializeObject(k);
                                var postResPerson = client.PostAsync(ApiEndpoints.Persons, new StringContent(newPersomJsonStr, Encoding.UTF8, "application/json")).Result;

                                var allPersonsID = await client.GetAsAsync<List<Person>>(ApiEndpoints.Persons);
                                var newPersonId = allPersonsID.FirstOrDefault(f => f.Name == k.Name).Id;
                                await AddFilmPerson(client, newFilmId, newPersonId, FilmPersonRole.ExecutiveProducers);
                            }
                        }

                    }

                    var producer = driver.FindElements(By.XPath("//div[@data-attrid='kc:/film/film:producer']")).FirstOrDefault();
                    if (producer != null)
                    {
                        var newPerson = PersonParsingAsync(driver, FilmPersonRole.Producer).Result;

                        var allPersons = await client.GetAsAsync<List<Person>>(ApiEndpoints.Persons);
                        foreach (var k in newPerson)
                        {
                            var s = allPersons.FirstOrDefault(p => p.Name == k.Name);
                            if (s == null)
                            {
                                var newPersomJsonStr = JsonConvert.SerializeObject(k);
                                var postResPerson = client.PostAsync(ApiEndpoints.Persons, new StringContent(newPersomJsonStr, Encoding.UTF8, "application/json")).Result;

                                var allPersonsID = await client.GetAsAsync<List<Person>>(ApiEndpoints.Persons);
                                var newPersonId = allPersonsID.FirstOrDefault(f => f.Name == k.Name).Id;
                                await AddFilmPerson(client, newFilmId, newPersonId, FilmPersonRole.Producer);
                            }
                        }

                    }

                    var screenplay = driver.FindElements(By.XPath("//div[@data-attrid='kc:/film/film:screenplay']")).FirstOrDefault();
                    if (screenplay != null)
                    {
                        var newPerson = PersonParsingAsync(driver, FilmPersonRole.Screenplay).Result;

                        var allPersons = await client.GetAsAsync<List<Person>>(ApiEndpoints.Persons);
                        foreach (var k in newPerson)
                        {
                            var s = allPersons.FirstOrDefault(p => p.Name == k.Name);
                            if (s == null)
                            {
                                var newPersomJsonStr = JsonConvert.SerializeObject(k);
                                var postResPerson = client.PostAsync(ApiEndpoints.Persons, new StringContent(newPersomJsonStr, Encoding.UTF8, "application/json")).Result;

                                var allPersonsID = await client.GetAsAsync<List<Person>>(ApiEndpoints.Persons);
                                var newPersonId = allPersonsID.FirstOrDefault(f => f.Name == k.Name).Id;
                                await AddFilmPerson(client, newFilmId, newPersonId, FilmPersonRole.Screenplay);
                            }
                        }
                    }

                    var storyby = driver.FindElements(By.XPath("//div[@data-attrid='kc:/film/film:story']")).FirstOrDefault();
                    if (storyby != null)
                    {
                        var newPerson = PersonParsingAsync(driver, FilmPersonRole.StoryBy).Result;

                        var allPersons = await client.GetAsAsync<List<Person>>(ApiEndpoints.Persons);
                        foreach (var k in newPerson)
                        {
                            var s = allPersons.FirstOrDefault(p => p.Name == k.Name);
                            if (s == null)
                            {
                                var newPersomJsonStr = JsonConvert.SerializeObject(k);
                                var postResPerson = client.PostAsync(ApiEndpoints.Persons, new StringContent(newPersomJsonStr, Encoding.UTF8, "application/json")).Result;

                                var allPersonsID = await client.GetAsAsync<List<Person>>(ApiEndpoints.Persons);
                                var newPersonId = allPersonsID.FirstOrDefault(f => f.Name == k.Name).Id;
                                await AddFilmPerson(client, newFilmId, newPersonId, FilmPersonRole.StoryBy);
                            }
                        }
                    }

                    //    newFilm.ProductionCompanies = driver
                    //        .FindElement(By.XPath("//div[@data-attrid = 'kc:/film/film:production company']")).Text


                    var cast = driver.FindElements(By.XPath("//div[@data-attrid = 'kc:/film/film:cast']//div//a")).FirstOrDefault();
                    if (cast != null)
                    {
                        var newPerson = PersonParsingAsync(driver, FilmPersonRole.Cast).Result;

                        var allPersons = await client.GetAsAsync<List<Person>>(ApiEndpoints.Persons);
                        foreach (var k in newPerson)
                        {
                            var s = allPersons.FirstOrDefault(p => p.Name == k.Name);
                            if (s == null)
                            {
                                var newPersomJsonStr = JsonConvert.SerializeObject(k);
                                var postResPerson = client.PostAsync(ApiEndpoints.Persons, new StringContent(newPersomJsonStr, Encoding.UTF8, "application/json")).Result;

                                var allPersonsID = await client.GetAsAsync<List<Person>>(ApiEndpoints.Persons);
                                var newPersonId = allPersonsID.FirstOrDefault(f => f.Name == k.Name).Id;
                                await AddFilmPerson(client, newFilmId, newPersonId, FilmPersonRole.Cast);
                            }
                        }
                    }
                }                         


            }
        }

        private static async Task<List<Person>> PersonParsingAsync(ChromeDriver driver, FilmPersonRole role)
        {
            List<Person> people = new List<Person>();
            if (role == FilmPersonRole.Director)
            {
                var navigateToDirectorTab = driver.FindElements(By.XPath("//div[@data-attrid = 'kc:/film/film:director']//span//a")).FirstOrDefault();
                if (navigateToDirectorTab != null)
                {
                    var listInfo = driver.FindElements(By.XPath("//div[@data-attrid = 'kc:/film/film:director']//span[2]//a"));
                    List<string> likslist = new List<string>();
                    foreach (var p in listInfo)
                    {
                        var link = p.GetAttribute("href");
                        likslist.Add(link);
                    }

                    foreach (var p in likslist)
                    {
                        Person newPerson = new Person();
                        driver.ExecuteScript("window.open();");
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                        driver.Navigate().GoToUrl(p);
                        if (likslist.Count == 1)
                        {
                            var navigate = driver.FindElements(By.XPath("//div[@data-attrid='kc:/film/film:director']//div//a")).FirstOrDefault().GetAttribute("href");
                            driver.Navigate().GoToUrl(navigate);
                        }


                        Thread.Sleep(1500);
                        newPerson.Description = driver.FindElements(By.XPath("//div[@data-attrid='description']")).FirstOrDefault()?.Text.Replace("Опис","");
                        newPerson.BirthnIformation = driver.FindElements(By.XPath("//div[@data-attrid='kc:/people/person:born']")).FirstOrDefault()?.Text.Replace("Народження:","");
                        newPerson.Name = driver.FindElements(By.XPath("//h2[@data-attrid='title']")).FirstOrDefault()?.Text;
                        var socialNetworksLinksChecked = driver.FindElements(By.XPath("//div[@data-attrid='kc:/common/topic:social media presence']//a")).FirstOrDefault();
                        if (socialNetworksLinksChecked != null)
                        {
                            List<string> socialLinks = new List<string>();
                            foreach (var link in driver.FindElements(By.XPath("//div[@data-attrid='kc:/common/topic:social media presence']//a")))
                            {
                                socialLinks.Add(link.GetAttribute("href"));
                            }
                            newPerson.SocialNetworksLinks = string.Join(",", socialLinks);
                        }
                        var ImgUrl = driver.FindElements(By.XPath("//g-img//img[@height ='186']")).FirstOrDefault();
                        if (ImgUrl != null)
                        {
                            newPerson.ImgUrl = "/img/" + newPerson.Name + ".jpg";
                            await Task.Run(() => TakePersonScreenshot(driver, ImgUrl, newPerson));
                        }
                        var secondImgUrl = driver.FindElements(By.XPath("//g-img[@data-attrid]//img")).FirstOrDefault();
                        if (secondImgUrl != null)
                        {
                            newPerson.ImgUrl = "/img/" + newPerson.Name + ".jpg";
                            await Task.Run(() => TakePersonScreenshot(driver, secondImgUrl, newPerson));

                        }
                        driver.Close();
                        driver.SwitchTo().Window(driver.WindowHandles.FirstOrDefault());
                        people.Add(newPerson);
                    }
                }
            }
            if (role == FilmPersonRole.Producer)
            {
                var navigateToDirectorTab = driver.FindElements(By.XPath("//div[@data-attrid = 'kc:/film/film:producer']//span//a"));
                if (navigateToDirectorTab != null)
                {
                    var listInfo = driver.FindElements(By.XPath("//div[@data-attrid = 'kc:/film/film:producer']//span[2]//a"));
                    List<string> likslist = new List<string>();
                    foreach (var p in listInfo)
                    {
                        var link = p.GetAttribute("href");
                        likslist.Add(link);
                    }
                    foreach (var p in likslist)
                    {
                        Person newPerson = new Person();
                        driver.ExecuteScript("window.open();");
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                        driver.Navigate().GoToUrl(p);
                        if (likslist.Count == 1)
                        {
                            var navigate = driver.FindElements(By.XPath("//div[@data-attrid='kc:/film/film:producer']//div//a")).FirstOrDefault().GetAttribute("href");
                            driver.Navigate().GoToUrl(navigate);
                        }

                        Thread.Sleep(1500);
                        newPerson.Description = driver.FindElements(By.XPath("//div[@data-attrid='description']")).FirstOrDefault()?.Text;
                        newPerson.BirthnIformation = driver.FindElements(By.XPath("//div[@data-attrid='kc:/people/person:born']")).FirstOrDefault()?.Text;
                        newPerson.Name = driver.FindElements(By.XPath("//h2[@data-attrid='title']")).FirstOrDefault()?.Text;
                        var socialNetworksLinksChecked = driver.FindElements(By.XPath("//div[@data-attrid='kc:/common/topic:social media presence']//a")).FirstOrDefault();
                        if (socialNetworksLinksChecked != null)
                        {
                            List<string> socialLinks = new List<string>();
                            foreach (var link in driver.FindElements(By.XPath("//div[@data-attrid='kc:/common/topic:social media presence']//a")))
                            {
                                socialLinks.Add(link.GetAttribute("href"));
                            }
                            newPerson.SocialNetworksLinks = string.Join(",", socialLinks);
                        }
                        var ImgUrl = driver.FindElements(By.XPath("//g-img//img[@height ='186']")).FirstOrDefault();
                        if (ImgUrl != null)
                        {
                            newPerson.ImgUrl = "/img/" + newPerson.Name + ".jpg";
                            await Task.Run(() => TakePersonScreenshot(driver, ImgUrl, newPerson));
                        }
                        var secondImgUrl = driver.FindElements(By.XPath("//g-img[@data-attrid]//img")).FirstOrDefault();
                        if (secondImgUrl != null)
                        {
                            newPerson.ImgUrl = "/img/" + newPerson.Name + ".jpg";
                            await Task.Run(() => TakePersonScreenshot(driver, secondImgUrl, newPerson));

                        }
                        people.Add(newPerson);
                        driver.Close();
                        driver.SwitchTo().Window(driver.WindowHandles.FirstOrDefault());                    

                    }

                }

            }
            if (role == FilmPersonRole.StoryBy)
            {
                var navigateToDirectorTab = driver.FindElements(By.XPath("//div[@data-attrid = 'kc:/film/film:story']//span[2]//a")).FirstOrDefault();
                if (navigateToDirectorTab != null)
                {
                    var listInfo = driver.FindElements(By.XPath("//div[@data-attrid = 'kc:/film/film:story']//span[2]//a"));
                    List<string> likslist = new List<string>();
                    foreach (var p in listInfo)
                    {
                        var link = p.GetAttribute("href");
                        likslist.Add(link);
                    }
                    foreach (var p in likslist)
                    {
                        Person newPerson = new Person();
                        driver.ExecuteScript("window.open();");
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                        driver.Navigate().GoToUrl(p);
                        if (likslist.Count == 1)
                        {
                            var navigate = driver.FindElements(By.XPath("//div[@data-attrid='kc:/film/film:story']//div//a")).FirstOrDefault().GetAttribute("href");
                            driver.Navigate().GoToUrl(navigate);
                        }

                        Thread.Sleep(1500);
                        newPerson.Description = driver.FindElements(By.XPath("//div[@data-attrid='description']")).FirstOrDefault()?.Text;
                        newPerson.BirthnIformation = driver.FindElements(By.XPath("//div[@data-attrid='kc:/people/person:born']")).FirstOrDefault()?.Text;
                        newPerson.Name = driver.FindElements(By.XPath("//h2[@data-attrid='title']")).FirstOrDefault()?.Text;
                        var socialNetworksLinksChecked = driver.FindElements(By.XPath("//div[@data-attrid='kc:/common/topic:social media presence']//a")).FirstOrDefault();
                        if (socialNetworksLinksChecked != null)
                        {
                            List<string> socialLinks = new List<string>();
                            foreach (var link in driver.FindElements(By.XPath("//div[@data-attrid='kc:/common/topic:social media presence']//a")))
                            {
                                socialLinks.Add(link.GetAttribute("href"));
                            }
                            newPerson.SocialNetworksLinks = string.Join(",", socialLinks);
                        }
                        var ImgUrl = driver.FindElements(By.XPath("//g-img//img[@height ='186']")).FirstOrDefault();
                        if (ImgUrl != null)
                        {
                            newPerson.ImgUrl = "/img/" + newPerson.Name + ".jpg";
                            await Task.Run(() => TakePersonScreenshot(driver, ImgUrl, newPerson));
                        }
                        var secondImgUrl = driver.FindElements(By.XPath("//g-img[@data-attrid]//img")).FirstOrDefault();
                        if (secondImgUrl != null)
                        {
                            newPerson.ImgUrl = "/img/" + newPerson.Name + ".jpg";
                            await Task.Run(() => TakePersonScreenshot(driver, secondImgUrl, newPerson));

                        }
                        people.Add(newPerson);
                        driver.Close();
                        driver.SwitchTo().Window(driver.WindowHandles.FirstOrDefault()); 
                    }
                }

            }
            if (role == FilmPersonRole.ExecutiveProducers)
            {
                var navigateToDirectorTab = driver.FindElements(By.XPath("//div[@data-attrid='kc:/film/film:executive producer']"));
                if (navigateToDirectorTab != null)
                {
                    var listInfo = driver.FindElements(By.XPath("//div[@data-attrid = 'kc:/film/film:executive producer']//span[2]//a"));
                    List<string> likslist = new List<string>();
                    foreach (var p in listInfo)
                    {
                        var link = p.GetAttribute("href");
                        likslist.Add(link);
                    }
                    foreach (var p in likslist)
                    {
                        Person newPerson = new Person();
                        driver.ExecuteScript("window.open();");
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                        driver.Navigate().GoToUrl(p);
                        if (likslist.Count == 1)
                        {
                            var navigate = driver.FindElements(By.XPath("//div[@data-attrid='kc:/film/film:executive producer']//div//a")).FirstOrDefault().GetAttribute("href");
                            driver.Navigate().GoToUrl(navigate);
                        }

                        Thread.Sleep(1500);
                        newPerson.Description = driver.FindElements(By.XPath("//div[@data-attrid='description']")).FirstOrDefault()?.Text;
                        newPerson.BirthnIformation = driver.FindElements(By.XPath("//div[@data-attrid='kc:/people/person:born']")).FirstOrDefault()?.Text;
                        newPerson.Name = driver.FindElements(By.XPath("//h2[@data-attrid='title']")).FirstOrDefault()?.Text;
                        var socialNetworksLinksChecked = driver.FindElements(By.XPath("//div[@data-attrid='kc:/common/topic:social media presence']//a")).FirstOrDefault();
                        if (socialNetworksLinksChecked != null)
                        {
                            List<string> socialLinks = new List<string>();
                            foreach (var link in driver.FindElements(By.XPath("//div[@data-attrid='kc:/common/topic:social media presence']//a")))
                            {
                                socialLinks.Add(link.GetAttribute("href"));
                            }
                            newPerson.SocialNetworksLinks = string.Join(",", socialLinks);
                        }
                        var ImgUrl = driver.FindElements(By.XPath("//g-img//img[@height ='186']")).FirstOrDefault();
                        if (ImgUrl != null)
                        {
                            newPerson.ImgUrl = "/img/" + newPerson.Name + ".jpg";
                            await Task.Run(() => TakePersonScreenshot(driver, ImgUrl, newPerson));
                        }
                        var secondImgUrl = driver.FindElements(By.XPath("//g-img[@data-attrid]//img")).FirstOrDefault();
                        if (secondImgUrl != null)
                        {
                            newPerson.ImgUrl = "/img/" + newPerson.Name + ".jpg";
                            await Task.Run(() => TakePersonScreenshot(driver, secondImgUrl, newPerson));

                        }
                        driver.Close();
                        driver.SwitchTo().Window(driver.WindowHandles.FirstOrDefault()); 
                        people.Add(newPerson);
                    }
                }

            }

            if (role == FilmPersonRole.Composer)
            {
                var navigateToDirectorTab = driver.FindElements(By.XPath("//div[@data-attrid='kc:/film/film:music']")).FirstOrDefault();
                if (navigateToDirectorTab != null)
                {
                    var listInfo = driver.FindElements(By.XPath("//div[@data-attrid = 'kc:/film/film:music']//span[2]//a"));
                    List<string> likslist = new List<string>();
                    foreach (var p in listInfo)
                    {
                        var link = p.GetAttribute("href");
                        likslist.Add(link);
                    }
                    foreach (var p in likslist)
                    {
                        Person newPerson = new Person();
                        driver.ExecuteScript("window.open();");
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                        driver.Navigate().GoToUrl(p);
                        if (likslist.Count == 1)
                        {
                            var navigate = driver.FindElements(By.XPath("//div[@data-attrid='kc:/film/film:music']//div//a")).FirstOrDefault().GetAttribute("href");
                            driver.Navigate().GoToUrl(navigate);
                        }

                        Thread.Sleep(1500);
                        newPerson.Description = driver.FindElements(By.XPath("//div[@data-attrid='description']")).FirstOrDefault()?.Text;
                        newPerson.BirthnIformation = driver.FindElements(By.XPath("//div[@data-attrid='kc:/people/person:born']")).FirstOrDefault()?.Text;
                        newPerson.Name = driver.FindElements(By.XPath("//h2[@data-attrid='title']")).FirstOrDefault()?.Text;
                        var socialNetworksLinksChecked = driver.FindElements(By.XPath("//div[@data-attrid='kc:/common/topic:social media presence']//a")).FirstOrDefault();
                        if (socialNetworksLinksChecked != null)
                        {
                            List<string> socialLinks = new List<string>();
                            foreach (var link in driver.FindElements(By.XPath("//div[@data-attrid='kc:/common/topic:social media presence']//a")))
                            {
                                socialLinks.Add(link.GetAttribute("href"));
                            }
                            newPerson.SocialNetworksLinks = string.Join(",", socialLinks);
                        }
                        var ImgUrl = driver.FindElements(By.XPath("//g-img//img[@height ='186']")).FirstOrDefault();
                        if (ImgUrl != null)
                        {
                            newPerson.ImgUrl = "/img/" + newPerson.Name + ".jpg";
                            await Task.Run(() => TakePersonScreenshot(driver, ImgUrl, newPerson));
                        }
                        var secondImgUrl = driver.FindElements(By.XPath("//g-img[@data-attrid]//img")).FirstOrDefault();
                        if (secondImgUrl != null)
                        {
                            newPerson.ImgUrl = "/img/" + newPerson.Name + ".jpg";
                            await Task.Run(() => TakePersonScreenshot(driver, secondImgUrl, newPerson));

                        }
                        driver.Close();
                        driver.SwitchTo().Window(driver.WindowHandles.FirstOrDefault()); 
                        people.Add(newPerson);
                    }
                }

            }


            if (role == FilmPersonRole.Screenplay)
            {
                var navigateToDirectorTab = driver.FindElements(By.XPath("//div[@data-attrid='kc:/film/film:screenplay']"));
                if (navigateToDirectorTab != null)
                {
                    var listInfo = driver.FindElements(By.XPath("//div[@data-attrid = 'kc:/film/film:screenplay']//span[2]//a"));
                    List<string> likslist = new List<string>();
                    foreach (var p in listInfo)
                    {
                        var link = p.GetAttribute("href");
                        likslist.Add(link);
                    }
                    foreach (var p in likslist)
                    {
                        Person newPerson = new Person();
                        driver.ExecuteScript("window.open();");
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                        driver.Navigate().GoToUrl(p);
                        if (likslist.Count == 1)
                        {
                            var navigate = driver.FindElements(By.XPath("//div[@data-attrid='kc:/film/film:screenplay']//div//a")).FirstOrDefault().GetAttribute("href");
                            driver.Navigate().GoToUrl(navigate);
                        }

                        Thread.Sleep(1500);
                        newPerson.Description = driver.FindElements(By.XPath("//div[@data-attrid='description']")).FirstOrDefault()?.Text;
                        newPerson.BirthnIformation = driver.FindElements(By.XPath("//div[@data-attrid='kc:/people/person:born']")).FirstOrDefault()?.Text;
                        newPerson.Name = driver.FindElements(By.XPath("//h2[@data-attrid='title']")).FirstOrDefault()?.Text;
                        var socialNetworksLinksChecked = driver.FindElements(By.XPath("//div[@data-attrid='kc:/common/topic:social media presence']//a")).FirstOrDefault();
                        if (socialNetworksLinksChecked != null)
                        {
                            List<string> socialLinks = new List<string>();
                            foreach (var link in driver.FindElements(By.XPath("//div[@data-attrid='kc:/common/topic:social media presence']//a")))
                            {
                                socialLinks.Add(link.GetAttribute("href"));
                            }
                            newPerson.SocialNetworksLinks = string.Join(",", socialLinks); 
                        }
                        var ImgUrl = driver.FindElements(By.XPath("//g-img//img[@height ='186']")).FirstOrDefault();
                        if (ImgUrl != null)
                        {
                            newPerson.ImgUrl = "/img/" + newPerson.Name + ".jpg";
                            await Task.Run(() => TakePersonScreenshot(driver, ImgUrl, newPerson));
                        }
                        var secondImgUrl = driver.FindElements(By.XPath("//g-img[@data-attrid]//img")).FirstOrDefault();
                        if (secondImgUrl != null)
                        {
                            newPerson.ImgUrl = "/img/" + newPerson.Name + ".jpg";
                            await Task.Run(() => TakePersonScreenshot(driver, secondImgUrl, newPerson));

                        }

                        driver.Close();
                        driver.SwitchTo().Window(driver.WindowHandles.FirstOrDefault()); 
                        people.Add(newPerson);
                    }
                }

            }

            if (role == FilmPersonRole.Cast)
            {
                var navigateToDirectorTab = driver.FindElements(By.XPath("//div[@data-attrid = 'kc:/film/film:cast']//div//a")).FirstOrDefault()?.GetAttribute("href");
                if (navigateToDirectorTab != null)
                {
                    driver.ExecuteScript("window.open();");
                    driver.SwitchTo().Window(driver.WindowHandles.Last());
                    driver.Navigate().GoToUrl(navigateToDirectorTab);
                    var listInfo = driver.FindElements(By.XPath("//a[@jscontroller = 'e8Ezlf']"));
                    List<string> likslist = new List<string>();
                    foreach (var p in listInfo)
                    {
                        var link = p.GetAttribute("href");
                        likslist.Add(link);
                    }

                    foreach (var p in likslist.Take(3))
                    {
                        Person newPerson = new Person();                        
                        driver.Navigate().GoToUrl(p);
                        if (likslist.Count == 1)
                        {
                            var navigate = driver.FindElements(By.XPath("//div[@data-attrid='kc:/film/film:music']//div//a")).FirstOrDefault().GetAttribute("href");
                            driver.Navigate().GoToUrl(navigate);
                        }

                        Thread.Sleep(1500);
                        newPerson.Description = driver.FindElements(By.XPath("//div[@data-attrid='description']")).FirstOrDefault()?.Text;
                        newPerson.BirthnIformation = driver.FindElements(By.XPath("//div[@data-attrid='kc:/people/person:born']")).FirstOrDefault()?.Text;
                        newPerson.Name = driver.FindElements(By.XPath("//h2[@data-attrid='title']")).FirstOrDefault()?.Text;
                        var socialNetworksLinksChecked = driver.FindElements(By.XPath("//div[@data-attrid='kc:/common/topic:social media presence']//a")).FirstOrDefault();
                        if (socialNetworksLinksChecked != null)
                        {
                            List<string> socialLinks = new List<string>();                     
                            foreach(var link in driver.FindElements(By.XPath("//div[@data-attrid='kc:/common/topic:social media presence']//a")))
                            {
                               socialLinks.Add(link.GetAttribute("href"));
                            }
                           newPerson.SocialNetworksLinks = string.Join(",", socialLinks); ;
                        }
                        var ImgUrl = driver.FindElements(By.XPath("//g-img//img[@height ='186']")).FirstOrDefault();
                        if (ImgUrl != null)
                        {
                            newPerson.ImgUrl = "/img/" + newPerson.Name + ".jpg";
                            await Task.Run(() => TakePersonScreenshot(driver, ImgUrl, newPerson));
                        }
                        var secondImgUrl = driver.FindElements(By.XPath("//g-img[@data-attrid]//img")).FirstOrDefault();
                        if (secondImgUrl != null)
                        {
                            newPerson.ImgUrl = "/img/" + newPerson.Name + ".jpg";
                            await Task.Run(() => TakePersonScreenshot(driver, secondImgUrl, newPerson));

                        }
                        
                        people.Add(newPerson);
                    }
                    driver.Close();
                    driver.SwitchTo().Window(driver.WindowHandles.FirstOrDefault());

                }

            }
            return people;
        }

        private static async Task AddFilmPerson(HttpClient client, int newFilmId, int newPersonId, FilmPersonRole role)
        {
            var allFilmPerson = await client.GetAsAsync<List<FilmPerson>>(ApiEndpoints.FilmPersons);


            FilmPerson filmPerson = new FilmPerson();
            filmPerson.FilmId = newFilmId;
            filmPerson.PersonId = newPersonId;
            filmPerson.Role = role;


            var newPersomFilmJsonStr = JsonConvert.SerializeObject(filmPerson);
            var postResFilmPerson = client.PostAsync(ApiEndpoints.FilmPersons, new StringContent(newPersomFilmJsonStr, Encoding.UTF8, "application/json")).Result;

        }

        public static void TakeFilmScreenshot(IWebDriver driver, IWebElement webElementImg, Film f)
        {

            string patch = @"E:\New folder (3)\Net CampMyProject\wwwroot\img\" + f.Title + ".jpg";

            byte[] byteArray = ((ITakesScreenshot)driver).GetScreenshot().AsByteArray;
            System.Drawing.Bitmap screenshot = new System.Drawing.Bitmap(new System.IO.MemoryStream(byteArray));
            System.Drawing.Rectangle croppedImage = new System.Drawing.Rectangle(webElementImg.Location.X, webElementImg.Location.Y, webElementImg.Size.Width, webElementImg.Size.Height);
            screenshot = screenshot.Clone(croppedImage, screenshot.PixelFormat);
            screenshot.Save(String.Format(patch, System.Drawing.Imaging.ImageFormat.Jpeg));
            patch = "/img/" + f.Title + ".jpg";            
        }
        public static void  TakePersonScreenshot(IWebDriver driver, IWebElement webElementImg, Person p)
        {

            string patch = @"E:\New folder (3)\Net CampMyProject\wwwroot\img\" + p.Name + ".jpg";

            byte[] byteArray = ((ITakesScreenshot)driver).GetScreenshot().AsByteArray;
            System.Drawing.Bitmap screenshot = new System.Drawing.Bitmap(new System.IO.MemoryStream(byteArray));
            System.Drawing.Rectangle croppedImage = new System.Drawing.Rectangle(webElementImg.Location.X, webElementImg.Location.Y, webElementImg.Size.Width, webElementImg.Size.Height);
            screenshot = screenshot.Clone(croppedImage, screenshot.PixelFormat);
            screenshot.Save(String.Format(patch, System.Drawing.Imaging.ImageFormat.Jpeg));           
        }

    }
}
