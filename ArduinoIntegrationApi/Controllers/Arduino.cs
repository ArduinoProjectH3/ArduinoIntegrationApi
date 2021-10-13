using Microsoft.AspNetCore.Mvc;
using ArduinoIntegrationApi.Context;
using Microsoft.Extensions.Configuration;
using ArduinoIntegrationApi.Authorization;


namespace ArduinoIntegrationApi.Controllers
{
    /// <summary>
    ///  this controller is used by the arduino to post data to the api
    /// </summary>
    [ApiController]
    [Route("api/Arduino")]
    public class Arduino : Controller

    {
        private IConfiguration Config;

        public Arduino(IConfiguration config)
        {
            this.Config = config;
            this.contextManager = new ContextManager(config);
        }

        private ContextManager contextManager;

        public ContextManager ContextManager
        {
            get { return contextManager; }
            set { contextManager = value; }
        }




        // this method is used by the arduino to post data
        [ApiKeyAuthorizer]
        [HttpPost]
        [Route("Post")]
        public StatusCodeResult PostRoomData(string roomName, float tempHead, float humHead, float tempFeet,
            string soundStatus, string curtainStatus, string lightStatus)
        {
            Authorizer authorizer = new Authorizer(Config);

            // check if the user is authorized with the correct api key


            // store the data in the database if the user is authorized
            if (ContextManager.PostRoomData(roomName, tempHead, humHead, tempFeet, soundStatus, curtainStatus,
                lightStatus))
            {
                return new StatusCodeResult(200);
            }


            return new StatusCodeResult(400);
        }
        
    }
}

