using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

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
                string sqlCheckUserExists =
                    "SELECT Email FROM dbo.Auth WHERE Email = '" + registerDto.Email + "'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);

                if (existingUsers.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(
                        registerDto.Password,
                        passwordSalt
                    );

                    string sqlAddAuth =
                        @"INSERT INTO dbo.Auth (Email,PasswordHash,PasswordSalt) VALUES ('"
                        + registerDto.Email
                        + "', @PasswordHash,@PasswordSalt)";

                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter passwordSaltParameter = new SqlParameter(
                        "@PasswordSalt",
                        SqlDbType.VarBinary
                    );
                    passwordSaltParameter.Value = passwordSalt;

                    SqlParameter passwordHashParameter = new SqlParameter(
                        "@PasswordHash",
                        SqlDbType.VarBinary
                    );
                    passwordHashParameter.Value = passwordHash;

                    sqlParameters.Add(passwordSaltParameter);
                    sqlParameters.Add(passwordHashParameter);

                    if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                    {
                        string sqlAddUser =
                            @"
                            INSERT INTO dbo.Users (
                                    [FirstName],
                                    [LastName],
                                    [Email],
                                    [Active]
                                ) VALUES ("
                            + "  '"
                            + registerDto.FirstName
                            + "',  '"
                            + registerDto.LastName
                            + "',  '"
                            + registerDto.Email
                            + "',1)";

                        if (_dapper.ExecuteSql(sqlAddUser))
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

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult LoginUser(LoginDto loginDto)
        {
            string sqlForHashAndSalt =
                @"
            SELECT 
            [PasswordHash],[PasswordSalt]
             FROM dbo.Auth 
             WHERE Email = '"
                + loginDto.Email
                + "'";

            LoginConfirmationDto userForConfirmation = _dapper.LoadDataSingle<LoginConfirmationDto>(
                sqlForHashAndSalt
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

            string sqlGetUserId =
                @"SELECT UserId 
            FROM dbo.Users 
            WHERE Email = '"
                + loginDto.Email
                + "'";

            int userId = _dapper.LoadDataSingle<int>(sqlGetUserId);

            return Ok(
                new Dictionary<string, string> { { "token", _authHelper.CreateToken(userId) } }
            );
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userId = User.FindFirst("userId")?.Value + "";

            string userIdSql = "SELECT UserId FROM dbo.Users Where UserId = " + userId;

            int userIdFromDb = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(
                new Dictionary<string, string>
                {
                    { "token", _authHelper.CreateToken(userIdFromDb) },
                }
            );
        }
    }
}
