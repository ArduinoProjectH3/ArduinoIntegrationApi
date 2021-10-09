using System;
using System.ComponentModel.DataAnnotations;


namespace ArduinoIntegrationApi.DataModels
{
    public class TemperatureReading
    {
        [Key] public int Tr_Id { get; set; }
        [Required] public float Tr_Value { get; set; }
    }
}