using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArduinoIntegrationApi.DataModels
{
    public class HumidityReading
    {
        [Key] public int Hr_Id { get; set; }
        [Required] public float Hr_Value { get; set; }
    }

}
