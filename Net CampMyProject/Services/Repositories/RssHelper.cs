using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Net_CampMyProject.Services.Results;

namespace Net_CampMyProject.Services
{
    public static class RssHelper
    {
        public static async Task<IEnumerable<RssItem>> GetFeedItemsAsync(string rssFeedUrl)
        {
            var result = new List<RssItem>();

            if (string.IsNullOrEmpty(rssFeedUrl))
                throw new ArgumentNullException(nameof(rssFeedUrl));

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(rssFeedUrl);
            if (!response.IsSuccessStatusCode)
                return result;

            using var reader = XmlReader.Create(await response.Content.ReadAsStreamAsync());
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(reader);

            var xmlNodeList = xmlDoc.SelectNodes("rss/channel/item");
            if (xmlNodeList == null)
                return result;

            foreach (XmlNode node in xmlNodeList)
            {
                var newItem = new RssItem();

                if (TryGetPropertyValue(node, "title", out newItem.Title) &&
                    TryGetPropertyValue(node, "description", out newItem.Description) && 
                    TryGetPropertyValue(node, "link", out newItem.Link) && 
                    TryGetPropertyValue(node, "fulltext", out newItem.Fulltext) && 
                    TryGetPropertyValue(node, "image", out newItem.Img) && 
                    TryGetPropertyValue(node, "pubDate", out var date) && 
                    DateTime.TryParse(date, out newItem.Date))
                {
                    result.Add(newItem);
                }
            }

            return result;
        }

        private static bool TryGetPropertyValue(XmlNode parent, string xPath, out string property)
        {
            var node = parent.SelectSingleNode(xPath);
            if (node != null)
            {
                property = node.InnerText;
                return true;
            }

            property = null;
            return false;
        }
    }
}
