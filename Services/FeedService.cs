using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using RSSFeedCacheWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace RSSFeedCacheWebApp.Services
{
    public interface IFeedService
    {
        Task<List<Article>> GetArticles();
    }
    public class FeedService : IFeedService
    {
        private static List<Article> _articleList = new List<Article>();
        private static DateTime _lastFetchDate;
        public async Task<List<Article>> GetArticles()
        {
            if (DateTime.Now > _lastFetchDate.AddHours(1))
                await readFeed();
            return _articleList;
        }

        private async Task readFeed()
        {
            using (XmlReader xmlReader = XmlReader.Create("https://blogs.msdn.microsoft.com/dotnet/feed/", new XmlReaderSettings() { Async = true }))
            {
                var reader = new RssFeedReader(xmlReader);

                while (await reader.Read())
                {
                    if (reader.ElementType == SyndicationElementType.Item)
                    {
                        var content = await reader.ReadContent();
                        _articleList.Add(mapArticle(content));
                    }
                }
            }
            _lastFetchDate = DateTime.Now;
        }

        private Article mapArticle(ISyndicationContent item)
        {
            var a = new Article();
            a.Id = item.Fields.FirstOrDefault(f => f.Name == "guid")?.Value;
            a.Title = item.Fields.FirstOrDefault(f => f.Name == "title")?.Value;
            a.Content = item.Fields.FirstOrDefault(f => f.Name == "encoded")?.Value;
            string pubDate = item.Fields.FirstOrDefault(f => f.Name == "pubDate")?.Value;
            if (DateTime.TryParse(pubDate, out DateTime date))
                a.DatePublished = date;
            a.Creator = item.Fields.FirstOrDefault(f => f.Name == "creator")?.Value;
            a.Description = item.Fields.FirstOrDefault(f => f.Name == "description")?.Value;
            Regex regex;
            regex = new Regex("<a[^>]+>Read more</a>");
            a.Description = regex.Replace(a.Description, "");

            regex = new Regex(@"src=""(https?://[^""]+)""", RegexOptions.Multiline);
            foreach (Match match in regex.Matches(a.Content))
            {
                var url = $"/Home/Resource?ResourceId={match.Groups[1].Value}";
                a.Content = a.Content.Replace(match.Value, $@"src=""{url}""");
                a.ResourcesUrl.Add(url);
            }
            return a;
        }
    }
}
