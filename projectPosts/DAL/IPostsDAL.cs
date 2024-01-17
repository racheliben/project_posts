using System;
using DTO;

namespace DAL
{
    public interface IPostsDAL
    {
      Task<Posts> GetNewsFeedAsync();
      Task<Post> GetPostDetailsAsync(string title);
    }
}
