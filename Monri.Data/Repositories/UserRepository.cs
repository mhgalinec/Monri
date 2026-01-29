using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Monri.Core.Models;
using Monri.Core.Models.DTOs;
using Monri.Data.Models;
using Monri.Data.Settings;

namespace Monri.Data.Repositories
{
    public interface IUserRepository
    {
        Task<Result<int>> InsertUser(UserDTO userDTO);
        Task<Result<int>> InsertUserWithDetails(UserDTO userDTO);
        Task<Result<bool>> CanInsertUser(UserDTO userDTO);
    }

    public class UserRepository : DatabaseRepository<User>, IUserRepository
    {
        public UserRepository(IOptions<ConnectionSettings> connectionSettings) : base(connectionSettings) { }

        public async Task<Result<int>> InsertUser(UserDTO userDTO)
        {
            string query = @"INSERT INTO USERS(FirstName, LastName, Email) VALUES (@FirstName, @LastName, @Email)";
            SqlParameter[] parameters =
            [
                new("@FirstName", userDTO?.FirstName?.Trim()),
                new("@LastName", userDTO?.LastName?.Trim()),
                new("@Email", userDTO?.Email)
            ];

            var result = await ExecuteNonQuery(query, parameters);
            return result == 0 ? Result.Failure<int>(Error.UnableToSave) : Result.Success(result);
        }

        public async Task<Result<bool>> CanInsertUser(UserDTO userDTO)
        {
            string query = @"SubmissionCheck";
            SqlParameter[] parameters =
            [
                new("@Email", userDTO.Email)
            ];

            var result = await ExecuteScalar(query, parameters, System.Data.CommandType.StoredProcedure);
            return result == 0 ? Result.Failure<bool>(Error.MaxEntriesReached) : Result.Success(true);
        }

        public async Task<Result<int>> InsertUserWithDetails(UserDTO userDTO)
        {
            string query = @"InsertUserWithDetails";
            SqlParameter[] parameters =
            [
                new("@Name", userDTO.Name),
                new("@FirstName", userDTO.FirstName),
                new("@LastName", userDTO.LastName),
                new("@Username", userDTO.Username),
                new("@Email", userDTO.Email),
                new("@Phone", userDTO.Phone),
                new("@Website", userDTO.Website),
                new("@Street", userDTO?.Address?.Street),
                new("@Suite", userDTO?.Address?.Suite),
                new("@City", userDTO?.Address?.City),
                new("@Zipcode", userDTO?.Address?.Zipcode),
                new("@Latitude", userDTO?.Address?.Geo.Lat),
                new("@Longitude", userDTO?.Address?.Geo.Lng),
                new("@CompanyName", userDTO?.Company?.Name),
                new("@CompanyCatchPhrase", userDTO?.Company?.CatchPhrase),
                new("@CompanyBs", userDTO?.Company?.Bs),
            ];

            var result = await ExecuteScalar(query, parameters, System.Data.CommandType.StoredProcedure);
            return result == 0 ? Result.Failure<int>(Error.UnableToSave) : Result.Success(result);
        }
    }
}
