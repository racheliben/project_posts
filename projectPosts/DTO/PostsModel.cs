namespace DTO
{
    public class Posts
    {
        public List<Post> postsDetails { get; set; }
    }

    public class Post
    {
        public string title { get; set; }
        public Body body { get; set; }
        public string link { get; set; }
    }

    public class Body
    {
        public string description { get; set; }
        public string audioUrl { get; set; }
        public string pubDate { get; set; }
        public List<string> categories { get; set; }

    }
    public class PostTitle
    {
        public string title { get; set; }
    } 
    public class PostsTitle
    {
     public  List<PostTitle> titles { get; set; }
    }
}