using System.ComponentModel.DataAnnotations;

namespace ArduinoIntegrationApi.DataModels
{
    /// <summary>
    /// this class is used hold data if the curtain is up or down
    /// </summary>
    public class CurtainReading
    {
        [Key] public int Cr_Id { get; set; }
        [MaxLength(10)]
        [Required] public string Cr_Value { get; set; }
    }
}