using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Monri.Core.Models;
using Monri.Data.Settings;
using System.Data;

namespace Monri.Data.Repositories
{
    public abstract class DatabaseRepository<T> where T : class
    {
        private readonly ConnectionSettings _connectionSettings;
        public DatabaseRepository(IOptions<ConnectionSettings> connectionSettings)
        {
            _connectionSettings = connectionSettings.Value;
        }

        protected async Task<int> ExecuteScalar(string query, SqlParameter[] parameters, CommandType cmdType = CommandType.Text)
        {
            using var conn = new SqlConnection(_connectionSettings.SQLConnectionString);
            using var cmd = new SqlCommand(query, conn);

            cmd.CommandType = cmdType;
            cmd.Parameters.AddRange(parameters);
            await conn.OpenAsync();

            var result = await cmd.ExecuteScalarAsync();
            if (result == null)
            {
                return 0;
            }
            return (int)result;
        }

        protected async Task<int> ExecuteNonQuery(string query, SqlParameter[] parameters, CommandType cmdType = CommandType.Text)
        {
            using var conn = new SqlConnection(_connectionSettings.SQLConnectionString);
            using var cmd = new SqlCommand(query, conn);

            cmd.CommandType = cmdType;
            cmd.Parameters.AddRange(parameters);
            await conn.OpenAsync();

            var result = await cmd.ExecuteNonQueryAsync();
            return result;
        }
    }
}

