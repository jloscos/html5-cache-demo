using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSFeedCacheWebApp.Models
{
    public class Article
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Creator { get; set; }
        public DateTime DatePublished { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string ExternalLink { get; set; }

        public List<string> ResourcesUrl { get; set; } = new List<string>();
    }
}
