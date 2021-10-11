using System;
using System.Security.Cryptography;
using ArduinoIntegrationApi.DataModels;

namespace ArduinoIntegrationApi.Authorization
{
    public static class Hasher
    {
        public static string[] SaltAndHashPassword(string password)
        {
            var saltByte = new byte[32];
            var rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(saltByte);
            var salt = Convert.ToBase64String(saltByte);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltByte, 1000);
            var hashedPassword = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));

            string[] saltAndPassword = new string[]
            {
                salt,
                hashedPassword
            };
            return saltAndPassword;
        }

        public static bool ValidatePassword(string password, Users user)
        {
            var saltBytes = Convert.FromBase64String(user.Salt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 1000);
            var hashedPassword = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));

            if (hashedPassword == user.Password)
            {
                return true;
            }

            return false;
        }
    }
}