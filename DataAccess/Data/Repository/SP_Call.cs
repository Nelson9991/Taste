using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Data.Repository.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data.Repository
{
    public class SP_Call : ISP_Call
    {
        private static string _connectionString = "";

        public SP_Call(ApplicationDbContext context)
        {
            _connectionString = context.Database.GetConnectionString();
        }

        public async Task<IEnumerable<T>> ReturnList<T>(string procName, DynamicParameters dynamicParameters = null)
        {
            await using SqlConnection sqlConnection = new SqlConnection(_connectionString);
            await sqlConnection.OpenAsync();
            return sqlConnection.Query<T>(procName, dynamicParameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task ExecuteWithoutResult(string procName, DynamicParameters dynamicParameters = null)
        {
            await using SqlConnection sqlConnection = new SqlConnection(_connectionString);
            await sqlConnection.OpenAsync();
            await sqlConnection.ExecuteAsync(procName, dynamicParameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<T> ExecuteReturnScalar<T>(string procName, DynamicParameters dynamicParameters = null)
        {
            await using SqlConnection sqlConnection = new SqlConnection(_connectionString);
            await sqlConnection.OpenAsync();
            return (T) Convert.ChangeType(sqlConnection.ExecuteScalar<T>(procName, dynamicParameters,
                commandType: CommandType.StoredProcedure), typeof(T));
        }

        public void Dispose()
        {
        }
    }
}