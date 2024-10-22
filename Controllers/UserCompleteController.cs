using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserCompleteController(IConfiguration config) : ControllerBase
    {
        private readonly DataContextDapper _dapper = new(config);

        [HttpGet("GetUsers/{userId}/{isActive}")]
        public IEnumerable<UserCompleteModel> GetUsers(int userId, bool isActive)
        {
            string sql = @"EXEC dbo.spUsers_Get";
            string parameters = "";

            if (userId != 0)
            {
                parameters += ", @UserId = " + userId.ToString();
            }

            if (isActive)
            {
                parameters += ", @Active= " + isActive.ToString();
            }

            sql += parameters.Substring(1);

            IEnumerable<UserCompleteModel> users = _dapper.LoadData<UserCompleteModel>(sql);

            return users;
        }

        [HttpPut("upsertUser")]
        public IActionResult UpsertUser(UserCompleteModel user)
        {
            string sql =
                @"
                EXEC dbo.spUser_Upsert 
                 @FirstName = '"
                + user.FirstName
                + "', @LastName = '"
                + user.LastName
                + "', @Email = '"
                + user.Email
                + "', @Active = '"
                + user.Active
                + "', @JobTitle = '"
                + user.JobTitle
                + "', @Department = '"
                + user.Department
                + "', @Salary = '"
                + user.Salary
                + "', @UserId = "
                + user.UserId;

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Update User");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql = @"EXEC dbo.spUser_DELETE @userId = " + userId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User");
        }
    }
}
