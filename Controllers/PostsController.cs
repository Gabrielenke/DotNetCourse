using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostsController(IConfiguration config) : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        [HttpGet("GetPosts/{postId?}/{userId?}/{searchParam?}")]
        public IEnumerable<PostModel> GetPosts(
            int? postId = 0,
            int? userId = 0,
            string? searchParam = "none"
        )
        {
            string sql = @"EXEC dbo.spPosts_Get";
            string parameters = "";

            DynamicParameters dynamicParameters = new();

            if (postId != 0)
            {
                parameters += ", @PostId = @PostIdParam ";
                dynamicParameters.Add("@PostIdParam", postId, DbType.Int32);
            }

            if (userId != 0)
            {
                parameters += ", @UserId = @UserIdParam";
                dynamicParameters.Add("@UserIdParam", userId, DbType.Int32);
            }

            if (searchParam != null && searchParam.ToLower() != "none")
            {
                parameters += ", @SearchValue = @SearchValueParam";
                dynamicParameters.Add("@SearchValueParam", searchParam, DbType.String);
            }

            if (parameters.Length > 0)
            {
                sql += parameters.Substring(1);
            }

            return _dapper.LoadDataWithParameters<PostModel>(sql, dynamicParameters);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<PostModel> GetMyPosts()
        {
            DynamicParameters dynamicParameters = new();

            dynamicParameters.Add("@UserIdParam", User.FindFirst("userId")?.Value, DbType.Int32);

            string sql = @"EXEC dbo.spPosts_Get @UserId = @UserIdParam ";

            return _dapper.LoadDataWithParameters<PostModel>(sql, dynamicParameters);
        }

        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(PostModel postToUpsert)
        {
            DynamicParameters dynamicParameters = new();

            dynamicParameters.Add(
                "@UserIdParameter",
                User.FindFirst("userId")?.Value,
                DbType.Int32
            );
            dynamicParameters.Add("@PostTitleParameter", postToUpsert.PostTitle, DbType.String);
            dynamicParameters.Add("@PostContentParameter", postToUpsert.PostContent, DbType.String);

            string sql =
                @"EXEC dbo.spPosts_Upsert 
                @UserId = @UserIdParameter,
                @PostTitle = @PostTitleParameter,
                @PostContent = @PostContentParameter";

            Console.WriteLine(sql);

            if (postToUpsert.PostId > 0)
            {
                sql += ", @PostId = @PostIdParameter";
                dynamicParameters.Add("@PostIdParameter", postToUpsert.PostId, DbType.Int32);
            }

            if (_dapper.ExecuteSqlWithParameters(sql, dynamicParameters))
            {
                return Ok();
            }

            throw new InvalidOperationException("Failed to upsert post!");
        }

        [HttpDelete("DeletePost/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            DynamicParameters dynamicParameters = new();
            dynamicParameters.Add(
                "@UserIdParameter",
                User.FindFirst("userId")?.Value,
                DbType.Int32
            );
            dynamicParameters.Add("@PostIdParameter", postId, DbType.Int32);

            string sql =
                @"EXEC dbo.spPost_Delete @PostId = @PostIdParameter ,
                @UserId = @UserIdParameter";

            if (_dapper.ExecuteSqlWithParameters(sql, dynamicParameters))
            {
                return Ok();
            }

            throw new InvalidOperationException("Failed to delete post!");
        }
    }
}
