using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserControllerEF : ControllerBase
    {
        private readonly DataContextEF _entityFramework;
        IMapper _mapper;

        public UserControllerEF(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);

            _mapper = new Mapper(
                new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<UserDto, UserModel>();
                })
            );
        }

        [HttpGet("GetUsers")]
        public IEnumerable<UserModel> GetUsers()
        {
            IEnumerable<UserModel> users = _entityFramework.Users.ToList();

            return users;
        }

        [HttpGet("GetSingleUser/{userId}")]
        public UserModel GetSingleUser(int userId)
        {
            UserModel? user = _entityFramework.Users.FirstOrDefault(u => u.UserId == userId);

            if (user != null)
            {
                return user;
            }

            throw new Exception("Failed to Get User");
        }

        [HttpPut("EditUser")]
        public IActionResult EditUser(UserModel user)
        {
            UserModel? userDb = _entityFramework.Users.FirstOrDefault(u => u.UserId == user.UserId);

            if (userDb != null)
            {
                userDb.FirstName = user.FirstName;
                userDb.LastName = user.LastName;
                userDb.Email = user.Email;
                userDb.Active = user.Active;

                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }
                throw new Exception("Failed to Update User");
            }

            throw new Exception("Failed to Update User");
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserDto user)
        {
            UserModel userDb = _mapper.Map<UserModel>(user);

            _entityFramework.Add(userDb);

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Create User");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            UserModel? userDb = _entityFramework.Users.FirstOrDefault(u => u.UserId == userId);

            if (userDb != null)
            {
                _entityFramework.Users.Remove(userDb);

                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }

                throw new Exception("Failed to Delete User");
            }

            throw new Exception("Failed to Get User");
        }

        [HttpGet("UserSalary/{userId}")]
        public IEnumerable<UserSalaryModel> GetUserSalaryEF(int userId)
        {
            return _entityFramework.UserSalary.Where(u => u.UserId == userId).ToList();
        }

        [HttpPost("UserSalary")]
        public IActionResult PostUserSalaryEf(UserSalaryModel userForInsert)
        {
            _entityFramework.UserSalary.Add(userForInsert);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Adding UserSalary failed on save");
        }

        [HttpPut("UserSalary")]
        public IActionResult PutUserSalaryEf(UserSalaryModel userForUpdate)
        {
            UserSalaryModel? userToUpdate = _entityFramework
                .UserSalary.Where(u => u.UserId == userForUpdate.UserId)
                .FirstOrDefault();

            if (userToUpdate != null)
            {
                _mapper.Map(userForUpdate, userToUpdate);
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }
                throw new Exception("Updating UserSalary failed on save");
            }
            throw new Exception("Failed to find UserSalary to Update");
        }

        [HttpDelete("UserSalary/{userId}")]
        public IActionResult DeleteUserSalaryEf(int userId)
        {
            UserSalaryModel? userToDelete = _entityFramework
                .UserSalary.Where(u => u.UserId == userId)
                .FirstOrDefault();

            if (userToDelete != null)
            {
                _entityFramework.UserSalary.Remove(userToDelete);
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }
                throw new Exception("Deleting UserSalary failed on save");
            }
            throw new Exception("Failed to find UserSalary to delete");
        }

        [HttpGet("UserJobInfo/{userId}")]
        public IEnumerable<UserJobInfoModel> GetUserJobInfoEF(int userId)
        {
            return _entityFramework.UserJobInfo.Where(u => u.UserId == userId).ToList();
        }

        [HttpPost("UserJobInfo")]
        public IActionResult PostUserJobInfoEf(UserJobInfoModel userForInsert)
        {
            _entityFramework.UserJobInfo.Add(userForInsert);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Adding UserJobInfo failed on save");
        }

        [HttpPut("UserJobInfo")]
        public IActionResult PutUserJobInfoEf(UserJobInfoModel userForUpdate)
        {
            UserJobInfoModel? userToUpdate = _entityFramework
                .UserJobInfo.Where(u => u.UserId == userForUpdate.UserId)
                .FirstOrDefault();

            if (userToUpdate != null)
            {
                _mapper.Map(userForUpdate, userToUpdate);
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }
                throw new Exception("Updating UserJobInfo failed on save");
            }
            throw new Exception("Failed to find UserJobInfo to Update");
        }

        [HttpDelete("UserJobInfo/{userId}")]
        public IActionResult DeleteUserJobInfoEf(int userId)
        {
            UserJobInfoModel? userToDelete = _entityFramework
                .UserJobInfo.Where(u => u.UserId == userId)
                .FirstOrDefault();

            if (userToDelete != null)
            {
                _entityFramework.UserJobInfo.Remove(userToDelete);
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }
                throw new Exception("Deleting UserJobInfo failed on save");
            }
            throw new Exception("Failed to find UserJobInfo to delete");
        }
    }
}
