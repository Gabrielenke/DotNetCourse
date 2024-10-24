using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController(IConfiguration config) : ControllerBase
    {
        private readonly DataContextDapper _dapper = new(config);
        private readonly ReusableSql _reusableSql = new(config);

        [HttpGet("GetUsers/{userId?}/{isActive?}")]
        public IEnumerable<UserModel> GetUsers(int userId, bool isActive)
        {
            string sql = @"EXEC dbo.spUsers_Get ";
            string parameters = "";

            DynamicParameters dynamicParameters = new();
            dynamicParameters.Add("@UserIdParam", userId, DbType.Int32);
            dynamicParameters.Add("@IsActive", isActive, DbType.Boolean);

            if (userId != 0)
            {
                parameters += ", @UserId = @UserIdParam";
            }
            if (isActive)
            {
                parameters += ", @Active = @IsActive";
            }

            if (parameters.Length > 0)
            {
                sql += parameters.Substring(1);
            }

            IEnumerable<UserModel> users = _dapper.LoadDataWithParameters<UserModel>(
                sql,
                dynamicParameters
            );
            return users;
        }

        [HttpPut("upsertUser")]
        public IActionResult UpsertUser(UserModel user)
        {
            if (_reusableSql.UpsertUser(user))
            {
                return Ok();
            }

            throw new InvalidOperationException("Failed to Update User");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            DynamicParameters dynamicParameters = new();
            dynamicParameters.Add("@UserIdParam", userId, DbType.Int32);

            string sql = @"EXEC dbo.spUser_DELETE @userId = @UserIdParam";

            if (_dapper.ExecuteSqlWithParameters(sql, dynamicParameters))
            {
                return Ok();
            }

            throw new InvalidOperationException("Failed to Delete User");
        }
    }
}
