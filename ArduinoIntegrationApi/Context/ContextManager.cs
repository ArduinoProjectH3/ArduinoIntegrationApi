using System;
using System.Collections.Generic;
using System.Linq;
using ArduinoIntegrationApi.Authorization;
using ArduinoIntegrationApi.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace ArduinoIntegrationApi.Context
{
    /// <summary>
    /// this class is used as a logic layer between the controllers and the ArduinoApiContext
    /// </summary>
    public class ContextManager
    {
        // property of ArduinoApiContext
        private ArduinoApiContext ctx;

        public ArduinoApiContext Ctx
        {
            get { return ctx; }
            set { ctx = value; }
        }


        private JwtAuthenticationManager jwtAuthenticationManager;

        public JwtAuthenticationManager JwtAuthenticationManager
        {
            get { return jwtAuthenticationManager; }
            set { jwtAuthenticationManager = value; }
        }

        // property of IConfiguration
        private IConfiguration config;

        public IConfiguration Config
        {
            get { return config; }
            set { config = value; }
        }

        public ContextManager(IConfiguration config)
        {
            this.Config = config;
        }


        // this method is used to to get the latest record of RoomReading from the database
        public RoomReading GetLatestRoomData(string roomName)
        {
            var allRoomData = GetAllRoomData();

            var latestRoomData = (from roomData in allRoomData
                where roomData.Rr_RoomName == roomName
                orderby roomData.Rr_Cts descending
                select roomData).ToList().FirstOrDefault();

            if (latestRoomData == null)
            {
                return null;
            }

            return latestRoomData;
        }

        // this method is used to get all records in RoomReading table
        private List<RoomReading> GetAllRoomData()
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

            if (allRoomData.Any())
            {
                return allRoomData;
            }

            return null;
        }

        // this method is used to insert a new Room data reading which comes from an Arduino
        public bool PostRoomData(string roomName, float tempHead, float humHead, float tempFeet,
            string soundStatus, string curtainStatus, string lightStatus)
        {
            bool newRoomDataAdded = false;

            DateTime dateNow = DateTime.Now;

            //// remove milliseconds from the timestamp
            dateNow = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, dateNow.Hour, dateNow.Minute,
                dateNow.Second, dateNow.Kind);

            using (ctx = new ArduinoApiContext())
            {
                // create a new RoomReading object
                ctx.RoomReading.Add(new RoomReading()
                {
                    Rr_Cts = dateNow,
                    Rr_RoomName = roomName,
                    // set Tr_Head object
                    Tr_Head = new TemperatureReading()
                    {
                        Tr_Value = tempHead,
                    },
                    // set Hr object
                    Hr = new HumidityReading()
                    {
                        Hr_Value = humHead
                    },
                    // set Tr_Feet object
                    Tr_Feet = new TemperatureReading()
                    {
                        Tr_Value = tempFeet
                    },
                    // set Sr ojbect 
                    Sr = new SoundReading()
                    {
                        Sr_Value = soundStatus
                    },
                    // set Cr object
                    Cr = new CurtainReading()
                    {
                        Cr_Value = curtainStatus
                    },
                    // set Lr object
                    Lr = new LightReading()
                    {
                        Lr_Value = lightStatus
                    }
                });
                // save the changes to the database
                ctx.SaveChanges();
                newRoomDataAdded = true;
            }

            return newRoomDataAdded;
        }

        // this method creates a new user in the database
        public bool CreateUser(User user)
        {
            ctx = new ArduinoApiContext();

            bool userCreated = false;
            // check if the user already exists
            if (GetUserFromDb(user.Username) == null)
            {
                // hash and salt the password provided by the user
                var saltAndHashedPassword = Hasher.SaltAndHashPassword(user.Password);
                // add the new user
                ctx.Users.Add(new Users()
                {
                    Username = user.Username,
                    Password = saltAndHashedPassword[1],
                    Salt = saltAndHashedPassword[0],
                    Email = user.Email
                });
                // save changes to the database
                ctx.SaveChanges();
                userCreated = true;
            }
            else
            {
                userCreated = false;
            }

            return userCreated;
        }

        // This method returns a potential user from the database with the provided username
        public Users GetUserFromDb(string userName)
        {
            ctx = new ArduinoApiContext();
            // return a user with the provided username 
            var potenitalUser = (from user in ctx.Users
                where user.Username == userName
                select user).FirstOrDefault();

            if (potenitalUser != null)
            {
                return potenitalUser;
            }

            return null;
        }

        // this method is used to verify the users username and password in the database
        public bool VerifyCredentials(Users potentialUser, string username, string password)
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

        // this method is used to save the token to the user int the database
        public string SaveTokenToUser(JwtToken token, Users potentialUser)
        {
            var oldToken = GetSpecificUserToken(potentialUser);

            if (potentialUser != null && oldToken != null)
            {
                return RefreshToken(oldToken.Token);
            }

            return CreateNewUserToken(token).Token;
        }

        // this method is used to create a new JwtToken in the database
        private JwtToken CreateNewUserToken(JwtToken newToken)
        {
            using (ctx = new ArduinoApiContext())
            {
                ctx.JwtTokens.Add(newToken);
                ctx.SaveChanges();
            }

            return newToken;
        }


        public JwtToken GetSpecificUserToken(Users potentialUser)
        {
            ctx = new ArduinoApiContext();
            var specificToken = (from token in ctx.JwtTokens
                where token.Username == potentialUser.Username
                select token).ToList().FirstOrDefault();
            return specificToken;
        }

        public string UpdateExistingToken(string oldToken, Users user, JwtToken newToken)
        {
            bool refreshed = false;

            using (ctx = new ArduinoApiContext())
            {
                var tokenToBeUpdated = ctx.JwtTokens.SingleOrDefault(jwt => jwt.Token == oldToken);
                if (tokenToBeUpdated != null)
                {
                    tokenToBeUpdated.Token = newToken.Token;
                    ctx.SaveChanges();
                    refreshed = true;
                }
            }

            if (refreshed)
            {
                return newToken.Token;
            }
            else
            {
                return null;
            }
        }

        // this method returns the user based on what token they have provided

        public Users GetUserFromDbWithToken(string token)
        {
            ctx = new ArduinoApiContext();
            var potentialUser = (from usr in ctx.Users
                join tok in ctx.JwtTokens on usr.Username equals tok.Username
                where tok.Token == token
                select usr).ToList().FirstOrDefault();

            if (potentialUser == null)
            {
                return null;
            }

            return potentialUser;
        }

        public string RefreshToken(string oldToken)
        {
            jwtAuthenticationManager = new JwtAuthenticationManager(config);
            DateTime expiryDate = DateTime.Now.AddMinutes(30);

            var potentialUser = GetUserFromDbWithToken(oldToken);

            if (potentialUser != null)
            {
                JwtToken newToken = jwtAuthenticationManager.GenerateToken(expiryDate, potentialUser);

                string newTok = UpdateExistingToken(oldToken, potentialUser, newToken);

                if (newTok != null)
                {
                    return newTok;
                }
            }

            return null;
        }
    }
}