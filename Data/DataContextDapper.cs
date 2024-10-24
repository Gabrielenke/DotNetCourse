using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Data
{
    class DataContextDapper
    {
        public string DefaultConnection = "DefaultConnection";
        private readonly IConfiguration _config;

        public DataContextDapper(IConfiguration config)
        {
            _config = config;
        }

        public bool ExecuteSql(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(
                _config.GetConnectionString(DefaultConnection)
            );

            return dbConnection.Execute(sql) > 0;
        }

        public int ExecuteSqlWithRowCount(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(
                _config.GetConnectionString(DefaultConnection)
            );

            return dbConnection.Execute(sql);
        }

        public bool ExecuteSqlWithParameters(string sql, DynamicParameters parameters)
        {
            IDbConnection dbConnection = new SqlConnection(
                _config.GetConnectionString(DefaultConnection)
            );

            return dbConnection.Execute(sql, parameters) > 0;
        }

        public IEnumerable<T> LoadDataWithParameters<T>(string sql, DynamicParameters parameters)
        {
            IDbConnection dbConnection = new SqlConnection(
                _config.GetConnectionString(DefaultConnection)
            );

            return dbConnection.Query<T>(sql, parameters);
        }

        public T LoadDataSingleWithParameters<T>(string sql, DynamicParameters parameters)
        {
            IDbConnection dbConnection = new SqlConnection(
                _config.GetConnectionString(DefaultConnection)
            );

            return dbConnection.QuerySingle<T>(sql, parameters);
        }
    }
}
