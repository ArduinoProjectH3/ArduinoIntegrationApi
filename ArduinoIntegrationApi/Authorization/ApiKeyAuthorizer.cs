using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace ArduinoIntegrationApi.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthorizer : Attribute, IAuthorizationFilter
    {
        private IConfiguration config;

        public IConfiguration Config
        {
            get { return config; }
            set { config = value; }
        }

        private Authorizer authorizer;

        public Authorizer Authorizer
        {
            get { return authorizer; }
            set { authorizer = value; }
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                authorizer = new Authorizer(config);
                if (!authorizer.ClientIsAuthorized(context.HttpContext.Request.Headers["apiKey"]))
                {
                    context.Result = new JsonResult(new {msg = "bad api key"})
                        {StatusCode = StatusCodes.Status401Unauthorized};
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}