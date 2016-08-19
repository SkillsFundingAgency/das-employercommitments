﻿using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        
        public UserRepository(IConfiguration configuration, ILogger logger) : base(configuration, logger)
        {
        }
        public async Task<User> GetById(string id)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userRef", new Guid(id), DbType.Guid);
                
                var res = await c.QueryAsync<User>(
                    sql: "SELECT Id, CONVERT(varchar(64), PireanKey) as UserRef, Email, FirstName, LastName FROM [dbo].[User] WHERE PireanKey = @userRef",
                    param: parameters,
                    commandType: CommandType.Text);
                return res;
            });
            return result.SingleOrDefault();
        }

        public async Task<User> GetByEmailAddress(string emailAddress)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@email", emailAddress, DbType.String);

                return await c.QueryAsync<User>(
                    sql: "SELECT Id, CONVERT(varchar(64), PireanKey) as UserRef, Email, FirstName, LastName FROM [dbo].[User] WHERE Email = @email",
                    param: parameters,
                    commandType: CommandType.Text);
            });
            return result.SingleOrDefault();
        }


        public async Task Create(User user)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@email", user.Email, DbType.String);
                parameters.Add("@userRef", new Guid(user.UserRef), DbType.Guid);
                parameters.Add("@firstName", user.FirstName, DbType.String);
                parameters.Add("@lastName", user.LastName, DbType.String);
                return await c.ExecuteAsync(
                    sql: "INSERT INTO [dbo].[User] (PireanKey, Email, FirstName, LastName) VALUES (@userRef, @email, @firstName, @lastName)",
                    param: parameters,
                    commandType: CommandType.Text);
            });
            Console.WriteLine(result);
        }

        public async Task Update(User user)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@email", user.Email, DbType.String);
                parameters.Add("@userRef", new Guid(user.UserRef), DbType.Guid);
                parameters.Add("@firstName", user.FirstName, DbType.String);
                parameters.Add("@lastName", user.LastName, DbType.String);
                return await c.ExecuteAsync(
                    sql: "UPDATE [dbo].[User] set Email = @email, FirstName = @firstName, LastName = @lastName where PireanKey = @userRef",
                    param: parameters,
                    commandType: CommandType.Text);
            });
            Console.WriteLine(result);
        }

        public async Task<Users> GetAllUsers()
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                return await c.QueryAsync<User>(
                    sql: "SELECT Id, CONVERT(varchar(64), PireanKey) as UserRef, Email, FirstName, LastName FROM [dbo].[User];",
                    param: parameters,
                    commandType: CommandType.Text);
            });
            return new Users
            {
                UserList = result.ToList()
            };
        }
    }
}
