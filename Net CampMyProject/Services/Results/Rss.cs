using System;

namespace Net_CampMyProject.Services.Results
{
    [Serializable]
    public struct RssItem
    {
        public DateTime Date;
        public string Title;
        public string Description;
        public string Link;
        public string Fulltext;
        public string Img;
    }
}