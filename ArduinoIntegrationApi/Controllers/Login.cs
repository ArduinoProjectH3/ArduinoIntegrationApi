using Microsoft.AspNetCore.Mvc;
using System;
using ArduinoIntegrationApi.Authorization;
using ArduinoIntegrationApi.Context;
using ArduinoIntegrationApi.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;


namespace ArduinoIntegrationApi.Controllers
{
    // this controller is used by angular to login a user and signup a user
    [ApiController]
    [Route("api/Login")]
    public class Login : Controller
    {
        // property of Iconfiguration
        private IConfiguration config;

        public IConfiguration Config
        {
            get { return config; }
            set { config = value; }
        }

        public Login(IConfiguration config)
        {
            this.config = config;
            this.contextManager = new ContextManager(config);
            this.jwtAuthenticationManager = new JwtAuthenticationManager(config);
        }

        // property of JwtAuthenticationManager
        private JwtAuthenticationManager jwtAuthenticationManager;

        public JwtAuthenticationManager JwtAuthenticationManager
        {
            get { return jwtAuthenticationManager; }
            set { jwtAuthenticationManager = value; }
        }

        private ContextManager contextManager;

        public ContextManager ContextManager
        {
            get { return contextManager; }
            set { contextManager = value; }
        }


        // post method is used to validate the users credentials and if they are correct, return a JwtToken string.
        [HttpPost]
        [Route("loginUser")]
        public IActionResult LoginUser([FromBody] User user)
        {
            try
            {

                // get a potentialuser from the database based on the provided username
                Users potentialUser = ContextManager.GetUserFromDb(user.Username);

                // check if the user exists
                if (potentialUser != null)
                {
                    // if the user exists in the database verify the username and password 
                    if (ContextManager.VerifyCredentials(potentialUser, user.Username, user.Password))
                    {
                        DateTime expiryDate = DateTime.Now.AddMinutes(30);
                        JwtToken tok = jwtAuthenticationManager.GenerateToken(expiryDate, potentialUser);
                        // store the token in the database and bind it to the username
                        string token = ContextManager.SaveTokenToUser(tok, potentialUser);

                        if (token != null)
                        {
                            // post the expiryDate in milliseconds
                            return Ok(new {Token = token, username = potentialUser.Username});
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return Unauthorized();
        }

        // this method is used by angular to create a new user in the database
        [AllowAnonymous]
        [HttpPost]
        [Route("Signup")]
        public IActionResult Signup([FromBody] User user)
        {
            // create the user
            if (ContextManager.CreateUser(user))
            {
                // return the username
                return Ok(new {name = user.Username});
            }

            return new BadRequestResult();
        }
    }
}