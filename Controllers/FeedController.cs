using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RSSFeedCacheWebApp.Services;

namespace RSSFeedCacheWebApp.Controllers
{
    public class FeedController : Controller
    {
        private IFeedService _feedService;
        public FeedController(IFeedService feedService)
        {
            _feedService = feedService;
        }
        public async Task<IActionResult> Index()
        {
            var items = await _feedService.GetArticles();
            return View(items);
        }

        public async Task<IActionResult> ArticleContent(string ArticleId)
        {
            var items = await _feedService.GetArticles();
            var article = items.Find(a => a.Id == ArticleId);
            if (article == null)
                return NotFound();
            return Content(article.Content, "text/html");
        }
    }
}