using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RSSFeedCacheWebApp.Services;

namespace RSSFeedCacheWebApp.Controllers
{
    public class HomeController : Controller
    {
        private IFeedService _feedService;
        private IMemoryCache _cache;
        public HomeController(IMemoryCache cache, IFeedService feedService)
        {
            _feedService = feedService;
            _cache = cache;
        }
        public async Task<IActionResult> AppCacheManifest()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("CACHE MANIFEST");
            sb.AppendLine("CACHE:");
            sb.AppendLine("/");
            sb.AppendLine("/Feed");
            sb.AppendLine("/favicon.ico");
            sb.AppendLine("/lib/bootstrap/dist/css/bootstrap.css");
            sb.AppendLine("/lib/jquery/dist/jquery.js");
            sb.AppendLine("/lib/bootstrap/dist/js/bootstrap.js");
            sb.AppendLine(_cache.Get("/css/site.css")?.ToString());
            sb.AppendLine(_cache.Get("/js/site.js")?.ToString());

            var articles = await _feedService.GetArticles();
            foreach(var article in articles)
            {
                sb.AppendLine("/Feed/ArticleContent?ArticleId=" + article.Id);
                foreach (var url in article.ResourcesUrl)
                    sb.AppendLine(url);
            }
            sb.AppendLine("# LastPublishedDate : " + articles.First().DatePublished.ToString());
            sb.AppendLine("NETWORK:");
            sb.AppendLine("*");

            return Content(sb.ToString(), "text/cache-manifest");
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Resource(string ResourceId)
        {
            var req = WebRequest.Create(ResourceId);
            using (var response = await req.GetResponseAsync())
            {
                using (var stream = response.GetResponseStream())
                {
                    var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    return File(ms, response.ContentType);               
                }
            }
        }
    }
}