using System.ComponentModel.DataAnnotations;

namespace ArduinoIntegrationApi.DataModels
{
    public class SoundReading
    {
        [Key] public int Sr_Id { get; set; }
        [MaxLength(10)]
        [Required] public string Sr_Value { get; set; }
    }
}