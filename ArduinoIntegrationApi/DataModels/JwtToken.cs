using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ArduinoIntegrationApi.DataModels
{
    public class JwtToken
    {
        [Key]public int JwtToken_Id { get; set; }

        [ForeignKey("Users")]public string Username { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
