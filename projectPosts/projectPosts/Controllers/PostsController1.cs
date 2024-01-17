using Microsoft.AspNetCore.Mvc;
using DAL;


namespace projectPosts.Controllers
{
    public class PostsController1 : Controller
    {
        private readonly ILogger<PostsController1> _logger;
        private readonly IPostsDAL _postsDAL;

        public PostsController1(ILogger<PostsController1> logger, IPostsDAL postsDAL)
        {
            _postsDAL = postsDAL;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _postsDAL.GetNewsFeedAsync();
            return View(posts);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string title)
        {
            var post =await _postsDAL.GetPostDetailsAsync(title);
            if (post == null)
            {
                return NotFound(); 
            }
            return View(post);
        }
    }
}
