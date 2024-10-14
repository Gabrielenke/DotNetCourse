using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public UserController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetUsers")]
        public IEnumerable<UserModel> GetUsers()
        {
            string sql =
                @"
                SELECT  [UserId]
                , [FirstName]
                , [LastName]
                , [Email]
                , [Active]
                FROM  dbo.Users;";

            IEnumerable<UserModel> users = _dapper.LoadData<UserModel>(sql);

            return users;
        }

        [HttpGet("GetSingleUser/{userId}")]
        public UserModel GetUsers(int userId)
        {
            string sql =
                @"
                SELECT  [UserId]
                , [FirstName]
                , [LastName]
                , [Email]
                , [Active]
                FROM  dbo.Users
                    WHERE UserId = " + userId.ToString();

            UserModel user = _dapper.LoadDataSingle<UserModel>(sql);

            return user;
        }

        [HttpPut("EditUser")]
        public IActionResult EditUser(UserModel user)
        {
            string sql =
                @"
                UPDATE dbo.Users 
                SET [FirstName] = '"
                + user.FirstName
                + "',[LastName] = '"
                + user.LastName
                + "',[Email] = '"
                + user.Email
                + "',[Active] = '"
                + user.Active
                + "' WHERE UserId = "
                + user.UserId;

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Update User");
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserDto user)
        {
            string sql =
                @"
             INSERT INTO dbo.Users (
                    [FirstName],
                    [LastName],
                    [Email],
                    [Active]
                ) VALUES ("
                + "  '"
                + user.FirstName
                + "',  '"
                + user.LastName
                + "',  '"
                + user.Email
                + "',  '"
                + user.Active
                + "')";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Register User");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql =
                @"
            DELETE FROM dbo.Users 
                WHERE userId = " + userId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User");
        }

        [HttpGet("UserSalary/{userId}")]
        public IEnumerable<UserSalaryModel> GetUserSalary(int userId)
        {
            return _dapper.LoadData<UserSalaryModel>(
                @"
            SELECT UserSalary.UserId
                    , UserSalary.Salary
            FROM  dbo.UserSalary
                WHERE UserId = " + userId.ToString()
            );
        }

        [HttpPost("UserSalary")]
        public IActionResult PostUserSalary(UserSalaryModel userSalaryForInsert)
        {
            string sql =
                @"
            INSERT INTO dbo.UserSalary (
                UserId,
                Salary
            ) VALUES ("
                + userSalaryForInsert.UserId.ToString()
                + ", "
                + userSalaryForInsert.Salary
                + ")";

            if (_dapper.ExecuteSqlWithRowCount(sql) > 0)
            {
                return Ok(userSalaryForInsert);
            }
            throw new Exception("Adding User Salary failed on save");
        }

        [HttpPut("UserSalary")]
        public IActionResult PutUserSalary(UserSalaryModel userSalaryForUpdate)
        {
            string sql =
                "UPDATE dbo.UserSalary SET Salary="
                + userSalaryForUpdate.Salary
                + " WHERE UserId="
                + userSalaryForUpdate.UserId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok(userSalaryForUpdate);
            }
            throw new Exception("Updating User Salary failed on save");
        }

        [HttpDelete("UserSalary/{userId}")]
        public IActionResult DeleteUserSalary(int userId)
        {
            string sql = "DELETE FROM dbo.UserSalary WHERE UserId=" + userId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Deleting User Salary failed on save");
        }

        [HttpGet("UserJobInfo/{userId}")]
        public IEnumerable<UserJobInfoModel> GetUserJobInfo(int userId)
        {
            return _dapper.LoadData<UserJobInfoModel>(
                @"
            SELECT  UserJobInfo.UserId
                    , UserJobInfo.JobTitle
                    , UserJobInfo.Department
            FROM  dbo.UserJobInfo
                WHERE UserId = " + userId.ToString()
            );
        }

        [HttpPost("UserJobInfo")]
        public IActionResult PostUserJobInfo(UserJobInfoModel userJobInfoForInsert)
        {
            string sql =
                @"
            INSERT INTO dbo.UserJobInfo (
                UserId,
                Department,
                JobTitle
            ) VALUES ("
                + userJobInfoForInsert.UserId
                + ", '"
                + userJobInfoForInsert.Department
                + "', '"
                + userJobInfoForInsert.JobTitle
                + "')";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok(userJobInfoForInsert);
            }
            throw new Exception("Adding User Job Info failed on save");
        }

        [HttpPut("UserJobInfo")]
        public IActionResult PutUserJobInfo(UserJobInfoModel userJobInfoForUpdate)
        {
            string sql =
                "UPDATE dbo.UserJobInfo SET Department='"
                + userJobInfoForUpdate.Department
                + "', JobTitle='"
                + userJobInfoForUpdate.JobTitle
                + "' WHERE UserId="
                + userJobInfoForUpdate.UserId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok(userJobInfoForUpdate);
            }
            throw new Exception("Updating User Job Info failed on save");
        }

        [HttpDelete("UserJobInfo/{userId}")]
        public IActionResult DeleteUserJobInfo(int userId)
        {
            string sql =
                @"
            DELETE FROM dbo.UserJobInfo 
                WHERE UserId = " + userId.ToString();

            Console.WriteLine(sql);

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User");
        }
    }
}
