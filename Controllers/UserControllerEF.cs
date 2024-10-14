using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using DotnetAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserControllerEF : ControllerBase
    {
        IUserRepository _userRepository;
        IMapper _mapper;

        public UserControllerEF(IConfiguration config, IUserRepository userRepository)
        {
            _userRepository = userRepository;

            _mapper = new Mapper(
                new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<UserDto, UserModel>();
                    cfg.CreateMap<UserSalaryModel, UserSalaryModel>();
                    cfg.CreateMap<UserJobInfoModel, UserJobInfoModel>();
                })
            );
        }

        [HttpGet("GetUsers")]
        public IEnumerable<UserModel> GetUsers()
        {
            IEnumerable<UserModel> users = _userRepository.GetUsers();

            return users;
        }

        [HttpGet("GetSingleUser/{userId}")]
        public UserModel GetSingleUser(int userId)
        {
            return _userRepository.GetSingleUser(userId);
        }

        [HttpPut("EditUser")]
        public IActionResult EditUser(UserModel user)
        {
            UserModel? userDb = _userRepository.GetSingleUser(user.UserId);

            if (userDb != null)
            {
                userDb.FirstName = user.FirstName;
                userDb.LastName = user.LastName;
                userDb.Email = user.Email;
                userDb.Active = user.Active;

                if (_userRepository.SaveChanges())
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

            _userRepository.AddEntity<UserModel>(userDb);

            if (_userRepository.SaveChanges())
            {
                return Ok();
            }

            throw new Exception("Failed to Create User");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            UserModel? userDb = _userRepository.GetSingleUser(userId);

            if (userDb != null)
            {
                _userRepository.RemoveEntity<UserModel>(userDb);

                if (_userRepository.SaveChanges())
                {
                    return Ok();
                }

                throw new Exception("Failed to Delete User");
            }

            throw new Exception("Failed to Get User");
        }

        [HttpGet("UserSalary/{userId}")]
        public UserSalaryModel GetUserSalaryEF(int userId)
        {
            return _userRepository.GetSingleUserSalary(userId);
        }

        [HttpPost("UserSalary")]
        public IActionResult PostUserSalaryEf(UserSalaryModel userForInsert)
        {
            _userRepository.AddEntity<UserSalaryModel>(userForInsert);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Adding UserSalary failed on save");
        }

        [HttpPut("UserSalary")]
        public IActionResult PutUserSalaryEf(UserSalaryModel userForUpdate)
        {
            UserSalaryModel? userToUpdate = _userRepository.GetSingleUserSalary(
                userForUpdate.UserId
            );

            if (userToUpdate != null)
            {
                _mapper.Map(userForUpdate, userToUpdate);
                if (_userRepository.SaveChanges())
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
            UserSalaryModel? userToDelete = _userRepository.GetSingleUserSalary(userId);

            if (userToDelete != null)
            {
                _userRepository.RemoveEntity<UserSalaryModel>(userToDelete);
                if (_userRepository.SaveChanges())
                {
                    return Ok();
                }
                throw new Exception("Deleting UserSalary failed on save");
            }
            throw new Exception("Failed to find UserSalary to delete");
        }

        [HttpGet("UserJobInfo/{userId}")]
        public UserJobInfoModel GetUserJobInfoEF(int userId)
        {
            return _userRepository.GetSingleUserJobInfo(userId);
        }

        [HttpPost("UserJobInfo")]
        public IActionResult PostUserJobInfoEf(UserJobInfoModel userForInsert)
        {
            _userRepository.AddEntity<UserJobInfoModel>(userForInsert);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Adding UserJobInfo failed on save");
        }

        [HttpPut("UserJobInfo")]
        public IActionResult PutUserJobInfoEf(UserJobInfoModel userForUpdate)
        {
            UserJobInfoModel? userToUpdate = _userRepository.GetSingleUserJobInfo(
                userForUpdate.UserId
            );

            if (userToUpdate != null)
            {
                _mapper.Map(userForUpdate, userToUpdate);
                if (_userRepository.SaveChanges())
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
            UserJobInfoModel? userToDelete = _userRepository.GetSingleUserJobInfo(userId);

            if (userToDelete != null)
            {
                _userRepository.RemoveEntity<UserJobInfoModel>(userToDelete);
                if (_userRepository.SaveChanges())
                {
                    return Ok();
                }
                throw new Exception("Deleting UserJobInfo failed on save");
            }
            throw new Exception("Failed to find UserJobInfo to delete");
        }
    }
}
