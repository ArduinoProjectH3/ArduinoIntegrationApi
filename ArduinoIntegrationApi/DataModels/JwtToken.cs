using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ArduinoIntegrationApi.DataModels
{
    public class JwtToken
    {
        
        [Key]public string Username { get; set; }
        public string Token { get; set; }
        
    }
}
