using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ArduinoIntegrationApi.DataModels;
using Microsoft.EntityFrameworkCore;


namespace ArduinoIntegrationApi.Context
{
    public static class ContextManager
    {
        public static RoomReading GetLatestRoomData(string roomName)
        {
            var allRoomData = GetAllRoomData();

            var latestRoomData = (from roomData in allRoomData
                where roomData.Rr_RoomName == roomName
                orderby roomData.Rr_Cts descending
                select roomData).ToList();

            return latestRoomData.FirstOrDefault();
        }


        private static List<RoomReading> GetAllRoomData()
        {
            ArduinoApiContext ctx = new ArduinoApiContext();
            var allRoomData = ctx.RoomReading
                .Include(temp => temp.Tr_Head)
                .Include(temp2 => temp2.Tr_Feet)
                .Include(light => light.Lr)
                .Include(sound => sound.Sr)
                .Include(Humidify => Humidify.Hr)
                .Include(curtain => curtain.Cr)
                .ToList();

            return allRoomData;
        }


        public static bool PostRoomData(string roomName, float tempHead, float humHead, float tempFeet,
            string soundStatus, string curtainStatus, string lightStatus)
        {
            bool newRoomDataAdded = false;

            DateTime dateNow = DateTime.Now;

            //// remove millisecounds
            dateNow = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, dateNow.Hour, dateNow.Minute,
                dateNow.Second, dateNow.Kind);

            using (ArduinoApiContext ctx = new ArduinoApiContext())
            {
                ctx.RoomReading.Add(new RoomReading()
                {
                    Rr_Cts = dateNow,
                    Rr_RoomName = roomName,
                    Tr_Head = new TemperatureReading()
                    {
                        Tr_Value = tempHead,
                    },
                    Hr = new HumidityReading()
                    {
                        Hr_Value = humHead
                    },
                    Tr_Feet = new TemperatureReading()
                    {
                        Tr_Value = tempFeet
                    },
                    Sr = new SoundReading()
                    {
                        Sr_Value = soundStatus
                    },
                    Cr = new CurtainReading()
                    {
                        Cr_Value = curtainStatus
                    },
                    Lr = new LightReading()
                    {
                        Lr_Value = lightStatus
                    }
                });
                Thread.Sleep(1000);
                ctx.SaveChanges();
                newRoomDataAdded = true;
            }

            return newRoomDataAdded;
        }
    }
}