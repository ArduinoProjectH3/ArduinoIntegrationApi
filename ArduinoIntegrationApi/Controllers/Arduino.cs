using Microsoft.AspNetCore.Mvc;
using ArduinoIntegrationApi.Context;
using Microsoft.Extensions.Configuration;
using ArduinoIntegrationApi.Authorization;


namespace ArduinoIntegrationApi.Controllers
{
    [ApiController]
    [Route("api/Arduino")]
    public class Arduino : Controller

    {

        private IConfiguration Config;

        public Arduino(IConfiguration config)
        {
            this.Config = config;

        }

        [HttpGet]
        [Route("Post")]
        public StatusCodeResult PostRoomData(string roomName, float tempHead, float humHead, float tempFeet,
            string soundStatus, string curtainStatus, string lightStatus)
        {
            Authorizer authorizer = new Authorizer(Config);

            bool userIsAuthorized = authorizer.ClientIsAuthorized(HttpContext.Request.Headers["apiKey"]);

            if (userIsAuthorized)
            {
                if (ContextManager.PostRoomData(roomName, tempHead, humHead, tempFeet, soundStatus, curtainStatus,
                    lightStatus))
                {
                    return new StatusCodeResult(200);
                }


                return new StatusCodeResult(400);
            }


            return new StatusCodeResult(401);
        }
    }
}