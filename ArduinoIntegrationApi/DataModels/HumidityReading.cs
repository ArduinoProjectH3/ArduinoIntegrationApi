using System.ComponentModel.DataAnnotations;


namespace ArduinoIntegrationApi.DataModels
{
    /// <summary>
    /// this class is used to hold the humidity percentage
    /// </summary>
    public class HumidityReading
    {
        [Key] public int Hr_Id { get; set; }
        [Required] public float Hr_Value { get; set; }
    }

}
