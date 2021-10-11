using System;
using System.Collections.Generic;
using System.Linq;
using ArduinoIntegrationApi.Authorization;
using ArduinoIntegrationApi.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace ArduinoIntegrationApi.Context
{
    public static class ContextManager
    {
        private static ArduinoApiContext ctx;

        public static ArduinoApiContext Ctx
        {
            get { return ctx; }
            set { ctx = value; }
        }

        private static JwtAuthenticationManager jwtAuthenticationManager;

        public static JwtAuthenticationManager JwtAuthenticationManager
        {
            get { return jwtAuthenticationManager; }
            set { jwtAuthenticationManager = value; }
        }

        private static IConfiguration config;

        public static IConfiguration Config
        {
            get { return config; }
            set { config = value; }
        }



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
            ctx = new ArduinoApiContext();
            var allRoomData = Ctx.RoomReading
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

            using (ctx = new ArduinoApiContext())
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
                ctx.SaveChanges();
                newRoomDataAdded = true;
            }

            return newRoomDataAdded;
        }

        public static bool CreateUser(User user)
        {
            ctx = new ArduinoApiContext();

            bool userCreated = false;
            if (GetUserFromDb(user.Username) == null)
            {
                var saltAndHashedPassword = Hasher.SaltAndHashPassword(user.Password);
                ctx.Users.Add(new Users()
                {
                    Username = user.Username,
                    Password = saltAndHashedPassword[1],
                    Salt = saltAndHashedPassword[0],
                    Email = user.Email
                });
                ctx.SaveChanges();
                userCreated = true;
            }
            else
            {
                userCreated = false;
            }

            return userCreated;
        }

        public static Users GetUserFromDb(string userName)
        {
            ctx = new ArduinoApiContext();
            return ctx.Users.FirstOrDefault(user => user.Username == userName);
        }

        public static bool VerifyCredentials(Users potentialUser, string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            if (potentialUser == null)
            {
                return false;
            }

            if (!Hasher.ValidatePassword(password, potentialUser))
            {
                return false;
            }

            return true;
        }

        public static JwtToken SaveTokenToUser(JwtToken token)
        {
            return CreateNewUserToken(token);
        }

        private static JwtToken CreateNewUserToken(JwtToken newToken)
        {
            using (ctx = new ArduinoApiContext())
            {
                ctx.JwtTokens.Add(newToken);
                ctx.SaveChanges();
            }

            return newToken;
        }

        private static bool UserHasToken(string tokenUsername)
        {
            ctx = new ArduinoApiContext();
            var potentialToken = (from token in ctx.JwtTokens
                where token.Username == tokenUsername
                select token).ToList().FirstOrDefault();

            if (potentialToken != null)
            {
                return true;
            }

            return false;
        }

        public static bool TokenInDbIsEqual(Users potentialUser, string? clientCookieToken)
        {
            ctx = new ArduinoApiContext();
            var tokenFromDb = (from token in ctx.JwtTokens
                where token.Username == potentialUser.Username && token.Token == clientCookieToken
                               select token).ToList().FirstOrDefault();

            if (tokenFromDb != null)
            {
                return true;
            }

            return false;
        }

        private static JwtToken GetSpecificUserToken(Users potentialUser)
        {
            ctx = new ArduinoApiContext();
            var specificToken = (from token in ctx.JwtTokens
                where token.Username == potentialUser.Username
                select token).ToList().FirstOrDefault();
            return specificToken;
        }

        public static bool RefreshToken(Users potentialUser, JwtToken newToken)
        {
            var tokenToBeUpdated = GetSpecificUserToken(potentialUser);

            using (ctx = new ArduinoApiContext())
            {
                tokenToBeUpdated.Token = newToken.Token;
                tokenToBeUpdated.ExpiryDate = newToken.ExpiryDate;
                ctx.SaveChanges();
                return true;
            }

            return false;
        }
    }
}