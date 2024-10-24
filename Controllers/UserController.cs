using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(IConfiguration config) : ControllerBase
    {
        private readonly DataContextDapper _dapper = new(config);

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
            DynamicParameters dynamicParameters = new();
            dynamicParameters.Add("@FirstNameParam", user.FirstName, DbType.String);
            dynamicParameters.Add("@LastNameParam", user.LastName, DbType.String);
            dynamicParameters.Add("@EmailParam", user.Email, DbType.String);
            dynamicParameters.Add("@ActiveParam", user.Active, DbType.Boolean);
            dynamicParameters.Add("@JobTitleParam", user.JobTitle, DbType.String);
            dynamicParameters.Add("@DepartmentParam", user.Department, DbType.String);
            dynamicParameters.Add("@SalaryParam", user.Salary, DbType.Decimal);
            dynamicParameters.Add("@UserIdParam", user.UserId, DbType.Int32);

            string sql =
                @"EXEC dbo.spUser_Upsert 
                @FirstName = @FirstNameParam ,
                @LastName = @LastNameParam ,
                @Email = @EmailParam,
                @Active = @ActiveParam,
                @JobTitle = @JobTitleParam,
                @Department = @DepartmentParam,
                @Salary = @SalaryParam,
                @UserId = @UserIdParam";

            if (_dapper.ExecuteSqlWithParameters(sql, dynamicParameters))
            {
                return Ok();
            }

            throw new Exception("Failed to Update User");
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

            throw new Exception("Failed to Delete User");
        }
    }
}
