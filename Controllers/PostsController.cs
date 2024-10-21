using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public PostsController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("Posts")]
        public IEnumerable<PostModel> GetPosts()
        {
            string sql =
                @"SELECT [PostId],
                        [UserId],
                        [PostTitle],
                        [PostContent],
                        [PostCreated],
                        [PostUpdated] 
                        FROM dbo.Posts";

            return _dapper.LoadData<PostModel>(sql);
        }

        [HttpGet("PostSingle/{postId}")]
        public PostModel GetSinglePosts(int postId)
        {
            string sql =
                @"SELECT [PostId],
                        [UserId],
                        [PostTitle],
                        [PostContent],
                        [PostCreated],
                        [PostUpdated] 
                        FROM dbo.Posts
                        WHERE PostId = " + postId.ToString();

            return _dapper.LoadDataSingle<PostModel>(sql);
        }

        [HttpGet("PostsByUser/{userId}")]
        public IEnumerable<PostModel> GetPostsByUser(int userId)
        {
            string sql =
                @"SELECT [PostId],
                        [UserId],
                        [PostTitle],
                        [PostContent],
                        [PostCreated],
                        [PostUpdated] 
                        FROM dbo.Posts
                        WHERE UserId = " + userId.ToString();

            return _dapper.LoadData<PostModel>(sql);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<PostModel> GetMyPosts()
        {
            string sql =
                @"SELECT [PostId],
                        [UserId],
                        [PostTitle],
                        [PostContent],
                        [PostCreated],
                        [PostUpdated] 
                        FROM dbo.Posts
                        WHERE UserId = " + User.FindFirst("userId")?.Value;

            return _dapper.LoadData<PostModel>(sql);
        }

        [HttpPost("Post")]
        public IActionResult addPost(PostToAddDto postToAdd)
        {
            string sql =
                @"INSERT INTO dbo.Posts (
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated]) VALUES ("
                + User.FindFirst("userId")?.Value
                + ",'"
                + postToAdd.PostTitle
                + "','"
                + postToAdd.PostContent
                + "',GETDATE(),GETDATE())";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to create new Post!");
        }

        [HttpPut("Post")]
        public IActionResult EditPost(PostToEditDto postToEdit)
        {
            string sql =
                @"
                UPDATE  dbo.Posts 
                SET PostContent = '"
                + postToEdit.PostContent
                + "', PostTitle = '"
                + postToEdit.PostTitle
                + @"', PostUpdated = GETDATE()
                WHERE PostId = "
                + postToEdit.PostId.ToString()
                + "AND UserId = "
                + User.FindFirst("userId")?.Value;

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to edit Post!");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql =
                "DELETE FROM dbo.Posts WHERE PostId = "
                + postId.ToString()
                + "AND UserId = "
                + User.FindFirst("userId")?.Value;

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to delete post!");
        }
    }
}
