using ArduinoIntegrationApi.Context;
using Microsoft.AspNetCore.Mvc;
using ArduinoIntegrationApi.DataModels;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;


namespace ArduinoIntegrationApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/angular")]
    public class Webpage : Controller
    {
        private IConfiguration config;

        public IConfiguration Config
        {
            get { return config; }
            set { config = value; }
        }

        public Webpage(IConfiguration config)
        {
            this.Config = config;
        }
       
        [HttpGet]
        [Route("GetRoom")]
        public string GetLatestRoomData(string roomName)
        {
            RoomReading latestRoomData = ContextManager.GetLatestRoomData(roomName);

            return JsonConvert.SerializeObject(latestRoomData);
        }
    }
}