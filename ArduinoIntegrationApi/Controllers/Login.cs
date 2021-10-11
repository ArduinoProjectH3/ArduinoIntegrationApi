using Microsoft.AspNetCore.Mvc;
using System;
using ArduinoIntegrationApi.Authorization;
using ArduinoIntegrationApi.Context;
using ArduinoIntegrationApi.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;


namespace ArduinoIntegrationApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/Login")]
    public class Login : Controller
    {
        private IConfiguration config;

        public IConfiguration Config
        {
            get { return config; }
            set { config = value; }
        }

        public Login(IConfiguration config)
        {
            this.config = config;
        }


        private JwtAuthenticationManager jwtAuthenticationManager;

        public JwtAuthenticationManager MyProperty
        {
            get { return jwtAuthenticationManager; }
            set { jwtAuthenticationManager = value; }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("loginUser")]
        public IActionResult LoginUser([FromBody] User user)
        {
            try {
                Users potentialUser = ContextManager.GetUserFromDb(user.Username);
                
                
                if (potentialUser != null)
                {
                    if (ContextManager.VerifyCredentials(potentialUser, user.Username, user.Password))
                    {
                        jwtAuthenticationManager = new JwtAuthenticationManager(config);

                        DateTime expiryDate = DateTime.Now.AddMinutes(30);

                        JwtToken token = jwtAuthenticationManager.GenerateToken(expiryDate, potentialUser);

                        ContextManager.SaveTokenToUser(token);

                        if (token != null)
                        { 
                            return Ok(new {Token = token.Token});
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

        [HttpPost]
        [Route("Signup")]
        public IActionResult Signup([FromBody] User user)
        {
            if (ContextManager.CreateUser(user))
            {
                return Ok(user.Username + "has been created");
            }

            return new BadRequestResult();
        }
    }
}