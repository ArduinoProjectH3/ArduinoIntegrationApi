using System;
using System.Security.Cryptography;
using ArduinoIntegrationApi.DataModels;

namespace ArduinoIntegrationApi.Authorization
{
    /// <summary>
    /// Hasher class contains logic to hash and salt a password
    /// </summary>
    public static class Hasher
    {
        // this method returns a string array containing a hashed password and a hashed salt.
        public static string[] SaltAndHashPassword(string password)
        
        { // create a byte array of 32 spaces long
            var saltByte = new byte[32];
            // create a new rng generator
            var rng = new RNGCryptoServiceProvider();
            // fill the saltByte with random numbers
            rng.GetNonZeroBytes(saltByte);
            // convert the saltBytes to a base64String
            var salt = Convert.ToBase64String(saltByte);
            // hash the password with the salt 1000 times
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltByte, 1000);
            // convert the hashedPassword to a base64String
            var hashedPassword = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));

            string[] saltAndPassword = new string[]
            {
                salt,
                hashedPassword
            };
            return saltAndPassword;
        }

        // this method is used to validate the input password from the user with the hashed password stored in the database
        public static bool ValidatePassword(string password, Users user)
        {
            // convert the salt from the database to a byte[]
            var saltBytes = Convert.FromBase64String(user.Salt);
            // hash the input password with the salt from the database 1000 times
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 1000);
            // convert the hashed input password to a base64String
            var hashedPassword = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));

            // check if the stored hashedPassword is equal to the newly hashedPassword
            if (hashedPassword == user.Password)
            {
                return true;
            }

            return false;
        }
    }
}