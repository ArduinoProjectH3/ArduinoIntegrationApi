using System;
using System.ComponentModel.DataAnnotations;

namespace ArduinoIntegrationApi.DataModels
{
    public class RoomReading
    {
        [MaxLength(30)]
        [Key] public string Rr_RoomName { get; set; }
        [Required]
        public DateTime Rr_Cts { get; set; }
        [Required]
        public virtual LightReading Lr { get; set; }
        [Required]
        public virtual TemperatureReading Tr_Head { get; set; }
        [Required]
        public virtual TemperatureReading Tr_Feet { get; set; }
        [Required]
        public virtual HumidityReading Hr { get; set; }
        [Required]
        public virtual CurtainReading Cr { get; set; }
        [Required]
        public virtual SoundReading Sr { get; set; }
    }
}