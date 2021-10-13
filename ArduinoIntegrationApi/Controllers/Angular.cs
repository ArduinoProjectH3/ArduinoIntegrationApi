using System;
using System.Collections.Generic;
using ArduinoIntegrationApi.Authorization;
using ArduinoIntegrationApi.Context;
using Microsoft.AspNetCore.Mvc;
using ArduinoIntegrationApi.DataModels;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;


namespace ArduinoIntegrationApi.Controllers
{
    /// <summary>
    /// this controller is used by angular to collect data
    /// </summary>
    [ApiController]
    [Route("api/angular")]
    public class Angular : Controller
    {
        // property of IConfiguration
        private IConfiguration config;

        public IConfiguration Config
        {
            get { return config; }
            set { config = value; }
        }

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

        public Angular(IConfiguration config)
        {
            this.Config = config;
            this.ContextManager = new ContextManager(config);
        }
        // this method uses JwtAuthorizer which will run everyTime there is a get request to this method.
        // if JwtAuthorizer returns an error then the method will not run
        //[JwtAuthorizer]
        [AllowAnonymous]
        [HttpGet]
        [Route("GetRoom")]
        public string GetLatestRoomData(string roomName, string token)
        {
            // create new token
            string newToken = contextManager.RefreshToken(token);
            // jsonString variable to hold the serialized RoomReading object
            string jsonString = null;
            // jsonObj is used to add an id to the json string
            string jsonObj = null;
            // JObject to represent the final jsonObject
            JObject result = null;

            if (newToken != null)
            {

                RoomReading latestRoomData = ContextManager.GetLatestRoomData(roomName);

                if (latestRoomData != null)
                {
                    jsonObj = "{data:[]}";
                    jsonString = JsonConvert.SerializeObject(latestRoomData);
                }
                result = JObject.Parse(jsonObj);
                result["data"] = jsonString;
                result["tokendata"] = JObject.Parse($@"{{""token"":""{newToken}""}}");
            }

            // remove backslashes from the json string
            return result.ToString().Replace("\\", "");

        }

        [AllowAnonymous]
        [HttpPost]
        [Route("RefreshToken")]
        public IActionResult RefreshToken([FromBody] string oldToken)
        {
            jwtAuthenticationManager = new JwtAuthenticationManager(config);
            // new expiryDate with plus 30 minutes
            DateTime expiryDate = DateTime.Now.AddMinutes(30);

            var potentialUser = ContextManager.GetUserFromDbWithToken(oldToken);

            if (potentialUser != null)
            {
                JwtToken newToken = jwtAuthenticationManager.GenerateToken(expiryDate, potentialUser);

                string newTok = ContextManager.UpdateExistingToken(oldToken, potentialUser, newToken);

                if (newTok !=null)
                {
                    return Ok(new { Token = newToken.Token });
                }
            }

            return BadRequest("something went wrong");
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ValidateToken")]
        public IActionResult ValidateToken(string username, string token)
        {
            var potentialUser = ContextManager.GetUserFromDbWithToken(token);
            var potentialUserToken = ContextManager.GetSpecificUserToken(potentialUser);

            if (potentialUser.Username == username && potentialUserToken.Token == token && potentialUserToken.Username == username)
            {
                return Ok("token is correct");
            }

            return Unauthorized("token is not correct");
        }
    }
}