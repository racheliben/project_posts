using System;
using DTO;

namespace DAL
{
    public interface IPostsDAL
    {
      Task<PostsTitle> GetNewsFeedAsync();
      Task<Post> GetPostDetailsAsync(string title);
    }
}
