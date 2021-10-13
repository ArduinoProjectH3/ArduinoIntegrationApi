using System;
using System.Linq;
using ArduinoIntegrationApi.Context;
using ArduinoIntegrationApi.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;


namespace ArduinoIntegrationApi.Authorization
{
    /// <summary>
    /// This class inherits from Attribute, IAuthorizationFilter.
    /// We are able to use this class to specify a custom authorization which will be used together with out JwtToken.
    /// If any method or classes has this class as [JwtAuthorizer] it will run the onAuthorization method first before the method
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JwtAuthorizer : Attribute, IAuthorizationFilter
    {

        private JwtAuthenticationManager jwtAuthenticationManager;

        public JwtAuthenticationManager JwtAuthenticationManager
        {
            get { return jwtAuthenticationManager; }
            set { jwtAuthenticationManager = value; }
        }

        private IConfiguration config;

        public IConfiguration Config
        {
            get { return config; }
            set { config = value; }
        }

        private ContextManager contextManager;

        public ContextManager ContextManager
        {
            get { return contextManager; }
            set { contextManager = value; }
        }


        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                contextManager = new ContextManager(config);
                jwtAuthenticationManager = new JwtAuthenticationManager(config); 
                // collect the token from the request header
                string token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (string.IsNullOrEmpty(token))
                {
                    context.Result = new JsonResult(new { msg = "no token has been found" })
                    { StatusCode = StatusCodes.Status401Unauthorized };
                }

                // get a potential user from the database which has the token assigned 

                Users user = ContextManager.GetUserFromDbWithToken(token);

                if (user == null)
                {
                    context.Result = new JsonResult(new { msg = "No user found" })
                    { StatusCode = StatusCodes.Status401Unauthorized };
                }

                // check if the expiration date on the token is expired compared to the time now.
                var dateNow = DateTime.Now;
                var tokenExpiryDate = Convert.ToDateTime(jwtAuthenticationManager.GetDecodedTokenExpiryDate(token));
              

                if (dateNow > tokenExpiryDate)
                {
                    context.Result = new JsonResult(new { msg = "token is expired"})
                        { StatusCode = StatusCodes.Status401Unauthorized };
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