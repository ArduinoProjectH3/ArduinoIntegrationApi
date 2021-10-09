using ArduinoIntegrationApi.Context;
using Microsoft.AspNetCore.Mvc;
using ArduinoIntegrationApi.DataModels;
using Newtonsoft.Json;


namespace ArduinoIntegrationApi.Controllers
{
    [ApiController]
    [Route("api/Webpage")]
    public class Webpage : Controller
    {
        [HttpGet]
        [Route("GetRoom")]
        public string GetLatestRoomData(string roomName)
        {
            RoomReading latestRoomData = ContextManager.GetLatestRoomData(roomName);

            return JsonConvert.SerializeObject(latestRoomData);
        }
    }
}