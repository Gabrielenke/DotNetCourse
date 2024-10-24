using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        private readonly AuthHelper _authHelper;

        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult RegisterUser(RegisterDto registerDto)
        {
            if (registerDto.Password == registerDto.PasswordConfirm)
            {
                DynamicParameters dynamicParameters = new DynamicParameters();

                dynamicParameters.Add("@EmailParam", registerDto.Email, DbType.String);

                string sqlCheckUserExists = "SELECT Email FROM dbo.Auth WHERE Email = @EmailParam";

                IEnumerable<string> existingUsers = _dapper.LoadDataWithParameters<string>(
                    sqlCheckUserExists,
                    dynamicParameters
                );

                if (existingUsers.Count() == 0)
                {
                    LoginDto userForSetPassword = new LoginDto()
                    {
                        Email = registerDto.Email,
                        Password = registerDto.Password,
                    };

                    if (_authHelper.SetPassword(userForSetPassword))
                    {
                        string sqlAddUser =
                            @"EXEC dbo.spUser_Upsert 
                                @FirstName = @FirstNameParam, 
                                @LastName = @LastNameParam, 
                                @Email = @EmailParam, 
                                @Active = 1, 
                                @JobTitle = @JobTitleParam, 
                                @Department = @DepartmentParam, 
                                @Salary = @SalaryParam";

                        dynamicParameters.Add(
                            "@FirstNameParam",
                            registerDto.FirstName,
                            DbType.String
                        );
                        dynamicParameters.Add(
                            "@LastNameParam",
                            registerDto.LastName,
                            DbType.String
                        );
                        dynamicParameters.Add(
                            "@JobTitleParam",
                            registerDto.JobTitle,
                            DbType.String
                        );
                        dynamicParameters.Add(
                            "@DepartmentParam",
                            registerDto.Department,
                            DbType.String
                        );
                        dynamicParameters.Add("@SalaryParam", registerDto.Salary, DbType.Decimal);

                        if (_dapper.ExecuteSqlWithParameters(sqlAddUser, dynamicParameters))
                        {
                            return Ok();
                        }
                        throw new Exception("Failed to Add User.");
                    }
                    throw new Exception("Failed to Register user.");
                }

                throw new Exception("User with this email already exists!");
            }

            throw new Exception("Password do not match!");
        }

        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(LoginDto userForSetPassword)
        {
            if (_authHelper.SetPassword(userForSetPassword))
            {
                return Ok();
            }

            throw new Exception("Failed to Update Password!");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult LoginUser(LoginDto loginDto)
        {
            string sqlForHashAndSalt = @"dbo.spLoginConfirmation_Get @Email = @EmailParam";

            DynamicParameters dynamicParameters = new DynamicParameters();

            dynamicParameters.Add("@EmailParam", loginDto.Email, DbType.String);

            LoginConfirmationDto userForConfirmation =
                _dapper.LoadDataSingleWithParameters<LoginConfirmationDto>(
                    sqlForHashAndSalt,
                    dynamicParameters
                );

            byte[] passwordHash = _authHelper.GetPasswordHash(
                loginDto.Password,
                userForConfirmation.PasswordSalt
            );

            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForConfirmation.PasswordHash[index])
                {
                    return StatusCode(401, "Incorrect Password!");
                }
            }

            string sqlGetUserId = @"SELECT UserId FROM dbo.Users WHERE Email = @EmailParam";

            int userId = _dapper.LoadDataSingleWithParameters<int>(sqlGetUserId, dynamicParameters);

            return Ok(
                new Dictionary<string, string> { { "token", _authHelper.CreateToken(userId) } }
            );
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            DynamicParameters dynamicParameters = new DynamicParameters();

            dynamicParameters.Add("@UserId", User.FindFirst("userId")?.Value, DbType.Int32);

            string userIdSql = "SELECT UserId FROM dbo.Users Where UserId = @UserId";

            int userIdFromDb = _dapper.LoadDataSingleWithParameters<int>(
                userIdSql,
                dynamicParameters
            );

            return Ok(
                new Dictionary<string, string>
                {
                    { "token", _authHelper.CreateToken(userIdFromDb) },
                }
            );
        }
    }
}
