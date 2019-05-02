using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using SecurityAPI.Services;
using SecurityAPI.Services.Interfaces;

namespace SecurityAPI
{
    public class VerifyAccess : ActionFilterAttribute
    {
        // private string FirstMessage;
        private bool _securityEnable;
        private readonly string _endpoint;
        private AccessConnectionPathsService _ACPathsService;
        public VerifyAccess(string endpoint)
        {
            _endpoint = endpoint;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IConfiguration _configuration = (IConfiguration)context.HttpContext.RequestServices.GetService(typeof(IConfiguration));
            _securityEnable = _configuration.GetSection("SecurityAPI").GetValue<bool>("enable");
            if (_securityEnable)
            {
                // Faz o parsing do Header procurando o perfil
                long idProfile;
                var profileGetterVerifier = GetValueFromHeader(context, "profile", out idProfile);

                // Faz o parsing do header procurando o usuário
                long idUser;
                var userGetterVerifier = GetValueFromHeader(context, "user", out idUser);

                if (profileGetterVerifier || userGetterVerifier)
                {
                    long idScreen;
                    var screenGetterVerifier = GetValueFromHeader(context, "screen", out idScreen);

                    if (screenGetterVerifier)
                    {
                        string connectionString = _configuration.GetSection("SecurityAPI").GetValue<string>("ConnectionString");

                        _ACPathsService = new AccessConnectionPathsService(connectionString);
                        bool getAccess;
                        string message;
                        // Se o perfil foi definido então valida por perfil
                        if (profileGetterVerifier)
                        {
                            (getAccess, message) = await _ACPathsService.VerifyProfileAccess(idProfile, idScreen, _endpoint);
                        }
                        // Se não, então, logicamente só chegamos aqui se o usuário foi definido  ¯\_(ツ)_/¯ (linha 35) 
                        else
                        {
                            (getAccess, message) = await _ACPathsService.VerifyUserAccess(idUser, idScreen,_endpoint);
                        }
                        if(getAccess){
                            Console.WriteLine("Ação permitida");
                            await next();
                        }
                        else{
                            SendResponse(context, message, 401);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Não conseguiu achar a tela - no header");
                        SendResponse(context, "Não conseguiu achar a tela - no header", 404);
                    }
                }
                else
                {
                    Console.WriteLine("Não conseguiu achar um perfil e nem um usuário válido - no header");
                    SendResponse(context, "Não conseguiu achar um perfil e nem um usuário válido - no header", 404);
                }
            }
            else Console.WriteLine("Segurança Desabilitada");
        }

        public void SendResponse(ActionExecutingContext context, string message, int statusCode)
        {
            context.HttpContext.Response.StatusCode = statusCode;
            context.HttpContext.Response.Headers.Clear();
            context.Result = new JsonResult(new { SecurityAPIMessage = message });
        }


        private bool GetValueFromHeader(ActionExecutingContext context, string key, out long value)
        {
            // Assinalando um valor padrão para poder retornar em caso de passar uma chave que não existe, ou a chave existe mas tem um valor de um tipo diferente de 'long'
            value = 0;

            // Tipo da variável que retorna do header;
            StringValues stringValues;

            //booleano que indica se o conseguiu resgatar um valor do header dado uma chave 'key'
            bool successRetrieving = context.HttpContext.Request.Headers.TryGetValue(key, out stringValues);

            if (!successRetrieving)
                return false;

            successRetrieving = Int64.TryParse(stringValues[0], out value);

            return successRetrieving;
        }
    }
}
