using System.Text;
using System.Xml;
using Microsoft.Extensions.Caching.Memory;
using DTO;
using System.Text.RegularExpressions;

namespace DAL
{
    public class PostsDAL:IPostsDAL
    {
        private readonly string URL= "https://www.93fm.co.il";
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;

        public PostsDAL(IMemoryCache cache, IHttpClientFactory httpClientFactory)
        {
            _cache = cache;
            _httpClientFactory = httpClientFactory;
        }
        
        public async Task<Posts> GetNewsFeedAsync()
        {
            var cacheData = _cache.Get<Posts>("Posts");
            if (cacheData != null)
            {
               return cacheData;
            }
            Posts posts = new Posts();
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.GetByteArrayAsync(URL+"/feed/");
                var xmlContent = Encoding.UTF8.GetString(response);
                xmlContent = xmlContent.TrimStart();
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);
                var entries = xmlDoc.SelectNodes("//item");
                posts = new Posts
                {
                    postsDetails = new List<Post>()
                };
                int i = 1;
                foreach (XmlNode entry in entries)
                {
                    var title = entry.SelectSingleNode("title").InnerText;
                    var descriptionoriginal = xmlDoc.SelectSingleNode("//item[" + i + "]/description")?.InnerText;
                    var description = Regex.Replace(descriptionoriginal, @"<[^>]+>|&quot;|&#039;|&#124;|&#8211|#;", string.Empty);
                    var link = entry.SelectSingleNode("link")?.InnerText;
                    XmlNodeList categoryesXml = entry.SelectNodes("category");
                    List<string> categories = new List<string>();
                    foreach (XmlNode item in categoryesXml)
                    {
                        categories.Add(item.InnerText);
                    }
                    var audioUrl = entry.SelectSingleNode("enclosure")?.Attributes["url"].Value;
                    var pubDate = entry.SelectSingleNode("pubDate")?.InnerText;
                    posts.postsDetails.Add(new Post 
                    { 
                        Title = title,
                        Body=new Body
                        {
                            description=description,
                            audioUrl=audioUrl,
                            pubDate=pubDate,
                            categories=categories
                        },
                        Link=URL+link
                    });
                    
                    i++;
                }
                var expirationTime = DateTimeOffset.Now.AddMinutes(15.0); 
                _cache.Set("Posts", posts, expirationTime);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return posts;
        }

        public async Task<Post> GetPostDetailsAsync(string title)
        {  
            Post currentPost = new Post();
            var cacheData = _cache.Get<Posts>("Posts");
            if (cacheData == null)
            {
                try
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    var response = await httpClient.GetByteArrayAsync("https://www.93fm.co.il/feed/");
                    var xmlContent = Encoding.UTF8.GetString(response);
                    xmlContent = xmlContent.TrimStart();
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlContent);
                    XmlNodeList entries = xmlDoc.SelectNodes("//item");
                    int i = 1;
                    foreach (XmlNode entry in entries)
                    {
                        string titleNew = entry.SelectSingleNode("title")?.InnerText;
                        if (title == titleNew)
                        {
                            XmlNode descriptionNode = entry.SelectSingleNode("//item:description");
                            var description = xmlDoc.SelectSingleNode("//item[" + i + "]/description")?.InnerText;
                            var link = entry.SelectSingleNode("link")?.InnerText;
                            XmlNodeList categoryesXml = entry.SelectNodes("category");
                            List<string> categories = new List<string>();
                            foreach (XmlNode item in categoryesXml)
                            {
                                categories.Add(item.InnerText);
                            }
                            var audioUrl = entry.SelectSingleNode("enclosure")?.Attributes["url"].Value;
                            var pubDate = entry.SelectSingleNode("pubDate")?.InnerText;
                            currentPost = new Post
                            {
                                Title = title,
                                Body = new Body
                                {
                                    description = description,
                                    audioUrl = audioUrl,
                                    pubDate = pubDate,
                                    categories = categories
                                },
                                Link = URL + link
                            }; 
                        }
                        i++;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                foreach (var post in cacheData.postsDetails)
                {
                    if (post.Title == title)
                        currentPost = post;
                }
            }
            return currentPost;
        }
    }
}



