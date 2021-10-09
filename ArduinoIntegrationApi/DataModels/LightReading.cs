using System;
using System.ComponentModel.DataAnnotations;


namespace ArduinoIntegrationApi.DataModels
{
    public class LightReading
    {
        [Key] public int Lr_Id { get; set; }
        [MaxLength(10)]
        [Required] public string Lr_Value { get; set; }
    }
}