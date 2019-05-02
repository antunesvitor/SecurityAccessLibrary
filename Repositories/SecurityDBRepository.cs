using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SecurityAPI.Models;

namespace SecurityAPI.Repositories
{
    public class SecurityDBRepository
    {
        private readonly string _connectionString;

        public SecurityDBRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<long>> GetEndpointIDsByScreenID(long id)
        {
            string query = $"SELECT [endpoint_id] FROM [PROFILE_ACCESS_CONTROL].[dbo].[screen_endpoint] WHERE screen_id = {id}";

            List<long> idsList;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (var commandExec = await conn.QueryMultipleAsync(query))
                {
                    idsList = (await commandExec.ReadAsync<long>()).ToList();
                }
            }

            return idsList;
        }

        public async Task<Screen> GetScreen(long id)
        {
            string query = $"SELECT * FROM [PROFILE_ACCESS_CONTROL].[dbo].[screen] WHERE id = {id}";

            Screen screen;
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                screen = await conn.QueryFirstOrDefaultAsync<Screen>(query);
            }

            return screen;
        }

        public Task<(bool, string)> VerifyUserAccess(long idProfile, long idScreen, string endpoint)
        {
            throw new System.NotImplementedException();
        }


        public async Task<long?> GetEndpointId(string endpointName)
        {

            string select = $"SELECT [id] FROM [PROFILE_ACCESS_CONTROL].[dbo].[endpoints] WHERE address = '{endpointName}'";

            long endpointIDs;

            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                endpointIDs = await conn.QueryFirstOrDefaultAsync<long>(select);

            }

            return endpointIDs;
        }

        public async Task<Profile> GetProfile(long id)
        {
            var query = $"SELECT * FROM [PROFILE_ACCESS_CONTROL].[dbo].[profile] WHERE [id] = {id}";

            Profile profile;
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                profile = await conn.QueryFirstOrDefaultAsync<Profile>(query);
            }

            return profile;
        }

        public async Task<User> GetUser(long id)
        {
            var query = $"SELECT * FROM [PROFILE_ACCESS_CONTROL].[dbo].[user] WHERE [id] = {id}";

            User user;
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                user = await conn.QueryFirstOrDefaultAsync<User>(query);
            }

            return user;
        }

        public async Task<List<long>> GetScreensIdsByProfileId(long idProfile)
        {
            string selectScreensByProfileIDstring = $"SELECT [screen_id] FROM [PROFILE_ACCESS_CONTROL].[dbo].[profile_screen] WHERE profile_id = {idProfile}";
            Console.WriteLine(selectScreensByProfileIDstring);
            List<long> selectScreensByProfileIDquery;
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {

                selectScreensByProfileIDquery = (await conn.QueryAsync<long>(selectScreensByProfileIDstring)).AsList();
            }

            return selectScreensByProfileIDquery;
        }

        public async Task<List<long>> GetScreensIdsByEndpointID(long id)
        {
            string selectScreensByProfileIDstring = $"SELECT [screen_id] FROM [PROFILE_ACCESS_CONTROL].[dbo].[screen_endpoint] WHERE endpoint_id = {id}";
            Console.WriteLine(selectScreensByProfileIDstring);
            List<long> selectScreensByProfileIDquery;
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {

                selectScreensByProfileIDquery = (await conn.QueryAsync<long>(selectScreensByProfileIDstring)).AsList();
            }

            return selectScreensByProfileIDquery;
        }
    }
}