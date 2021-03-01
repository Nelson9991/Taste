using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace DataAccess.Data.Repository.IRepository
{
    public interface ISP_Call : IDisposable
    {
        Task<IEnumerable<T>> ReturnList<T>(string procName, DynamicParameters dynamicParameters = null);
        Task ExecuteWithoutResult(string procName, DynamicParameters dynamicParameters = null);
        Task<T> ExecuteReturnScalar<T>(string procName, DynamicParameters dynamicParameters = null);
    }
}