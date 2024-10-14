using DotnetAPI.Data;
using DotnetAPI.Models;

namespace DotnetAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        DataContextEF _entityFramework;

        public UserRepository(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);
        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        public void AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entityFramework.Add(entityToAdd);
            }
        }

        public void RemoveEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entityFramework.Add(entityToAdd);
            }
        }

        public IEnumerable<UserModel> GetUsers()
        {
            IEnumerable<UserModel> users = _entityFramework.Users.ToList();

            return users;
        }

        public UserModel GetSingleUser(int userId)
        {
            UserModel? user = _entityFramework.Users.FirstOrDefault(u => u.UserId == userId);

            if (user != null)
            {
                return user;
            }

            throw new Exception("Failed to Get User");
        }

        public UserSalaryModel GetSingleUserSalary(int userId)
        {
            UserSalaryModel? userSalary = _entityFramework.UserSalary.FirstOrDefault(u =>
                u.UserId == userId
            );

            if (userSalary != null)
            {
                return userSalary;
            }

            throw new Exception("Failed to Get User");
        }

        public UserJobInfoModel GetSingleUserJobInfo(int userId)
        {
            UserJobInfoModel? userJobInfo = _entityFramework.UserJobInfo.FirstOrDefault(u =>
                u.UserId == userId
            );

            if (userJobInfo != null)
            {
                return userJobInfo;
            }

            throw new Exception("Failed to Get User");
        }
    }
}
