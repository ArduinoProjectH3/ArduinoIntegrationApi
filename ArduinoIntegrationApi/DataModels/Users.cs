using System.ComponentModel.DataAnnotations;


namespace ArduinoIntegrationApi.DataModels
{
    public class Users
    {
        [Key] [Required] public string Username { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }
        [Required] public string Salt { get; set; }
    }
}