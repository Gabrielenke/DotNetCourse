using System.Data;
using AutoMapper;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        private readonly AuthHelper _authHelper;

        private readonly ReusableSql _reusableSql;

        private readonly IMapper _mapper;

        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);

            _authHelper = new AuthHelper(config);

            _reusableSql = new ReusableSql(config);

            _mapper = new Mapper(
                new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<RegisterDto, UserModel>();
                })
            );
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

                if (!existingUsers.Any())
                {
                    LoginDto userForSetPassword = new LoginDto()
                    {
                        Email = registerDto.Email,
                        Password = registerDto.Password,
                    };

                    if (_authHelper.SetPassword(userForSetPassword))
                    {
                        UserModel user = _mapper.Map<UserModel>(registerDto);
                        user.Active = true;

                        if (_reusableSql.UpsertUser(user))
                        {
                            return Ok();
                        }
                        return StatusCode(500, "Failed to Add User.");
                    }
                    throw new InvalidOperationException("Failed to Register user.");
                }

                throw new InvalidOperationException("User with this email already exists!");
            }

            throw new ArgumentException("Passwords do not match!");
        }

        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(LoginDto userForSetPassword)
        {
            if (_authHelper.SetPassword(userForSetPassword))
            {
                return Ok();
            }

            return StatusCode(500, "Failed to Update Password!");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(typeof(Dictionary<string, string>), 200)]
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
        [ProducesResponseType(typeof(Dictionary<string, string>), 200)]
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
