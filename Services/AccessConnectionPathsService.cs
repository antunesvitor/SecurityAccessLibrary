using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using SecurityAPI.Models;
using SecurityAPI.Services.Interfaces;
using System.Linq;
using SecurityAPI.Repositories;

namespace SecurityAPI.Services
{
    public class AccessConnectionPathsService : IAccessConnectionPathsService
    {
        private readonly string _connectionString;

        private readonly SecurityDBRepository _securityDBRepository;
        public AccessConnectionPathsService(string connectionString)
        {
            _connectionString = connectionString;
            _securityDBRepository = new SecurityDBRepository(_connectionString);
        }
        public async Task<(bool, string)> VerifyProfileAccess(long idProfile, long idScreen, string endpoint)
        {
            Console.WriteLine("Entrou no serviço");
            Console.WriteLine(_connectionString);

            // Checar se a tela tem acesso ao endpoint

            // Se o perfil existe
            var profile = await _securityDBRepository.GetProfile(idProfile);

            if (profile == null)
                return (false, "Profile not Found");

            Console.WriteLine($"perfil: {profile.name} -> id: {idProfile}");

            // Se a tela existe
            var screen = await _securityDBRepository.GetScreen(idScreen);

            Console.WriteLine($"ID da tela: {idScreen}");

            if (screen == null)
                return (false, "Screen not Found");

            Console.WriteLine($"perfil: {screen.name} -> id: {idScreen}");

            // Se o endpoint existe 
            var endpointId = await _securityDBRepository.GetEndpointId(endpoint);

            if (endpointId == null)
                return (false, "Endpoint Not Found");

            Console.WriteLine($"endpoint: {endpoint} -> id: {endpointId}");

            // Busca uma lista de IDs de telas que podem ser acessadas pelo perfil
            var screensIDsByProfileID = await _securityDBRepository.GetScreensIdsByProfileId(idProfile);

            Console.WriteLine("Telas que podem ser acessadas pelo perfil:");
            screensIDsByProfileID.ForEach(x => Console.Write(x + " "));
            Console.WriteLine();

            if (!screensIDsByProfileID.Contains(screen.id))
                return (false, "The profile does not have access to the screen");


            var screensIDsByEndpointID = await _securityDBRepository.GetScreensIdsByEndpointID(endpointId.Value);

            Console.WriteLine("Telas que acessam esse Endpoint: ");
            screensIDsByEndpointID.ForEach(x => Console.Write(x + " "));
            Console.WriteLine();

            if (screensIDsByProfileID.Contains(idScreen) && screensIDsByEndpointID.Contains(idScreen))
                return (true, "Success");

            return (false, "Not allowed");
        }

        public async Task<(bool, string)> VerifyUserAccess(long idUser, long idScreen, string endpoint)
        {
            var user = await _securityDBRepository.GetUser(idUser);

            if(user == null)
                return (false, "User Not Found");

            Console.WriteLine($"User - id: {user.id} perfil: {user.profile_id}");
            
            var (profileAcessVerifier, message) = await VerifyProfileAccess(user.profile_id, idScreen, endpoint);

            return (profileAcessVerifier, message);
        }
    }
}