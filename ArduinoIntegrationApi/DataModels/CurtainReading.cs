using System.ComponentModel.DataAnnotations;

namespace ArduinoIntegrationApi.DataModels
{
    public class CurtainReading
    {
        [Key] public int Cr_Id { get; set; }
        [MaxLength(10)]
        [Required] public string Cr_Value { get; set; }
    }
}