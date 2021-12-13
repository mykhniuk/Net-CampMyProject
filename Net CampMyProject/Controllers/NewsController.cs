using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Net_CampMyProject.Models.ViewModels;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Net_CampMyProject.Services;

namespace Net_CampMyProject.Controllers
{
    public class NewsController : Controller 
    {
        private const string ImgComUaRssUrl = "http://k.img.com.ua/rss/ru/cinema.xml";

        public async Task<IActionResult> Index()
        {
            var rssItems = await RssHelper.GetFeedItemsAsync(ImgComUaRssUrl);

            var news = rssItems.Select(item => new NewsViewModel
                {
                    Title = item.Title,
                    Date = item.Date,
                    Description = Regex.Replace(item.Description, "<[^>]+>", string.Empty),
                    FullText = item.Fulltext,
                    NewsLink = item.Link,
                    ImgLink = item.Img.Split("=").LastOrDefault()?.Replace(">", "").Trim('"')
                })
                .ToList();

            return View(news);
        }
    }
}
