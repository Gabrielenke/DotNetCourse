using DotnetAPI.Data;
using DotnetAPI.Models;

namespace DotnetAPI.Repositories
{
    public interface IUserRepository
    {
        public bool SaveChanges();
        public void AddEntity<T>(T entityToAdd);
        public void RemoveEntity<T>(T entityToAdd);
        public IEnumerable<UserModel> GetUsers();
        public UserModel GetSingleUser(int userId);
        public UserSalaryModel GetSingleUserSalary(int userId);
        public UserJobInfoModel GetSingleUserJobInfo(int userId);
    }
}
