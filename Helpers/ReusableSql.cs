using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;

namespace DotnetAPI.Helpers
{
    public class ReusableSql
    {
        private readonly DataContextDapper _dapper;

        public ReusableSql(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        public bool UpsertUser(UserModel user)
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

            return _dapper.ExecuteSqlWithParameters(sql, dynamicParameters);
        }
    }
}
