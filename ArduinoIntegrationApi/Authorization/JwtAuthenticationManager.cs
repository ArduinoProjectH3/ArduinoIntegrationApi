using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using ArduinoIntegrationApi.DataModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ArduinoIntegrationApi.Authorization
{
    /// <summary>
    /// This class contains logic to generate and refresh a JWT token
    /// </summary>
    public class JwtAuthenticationManager
    {
        // property of Iconfiguration used to be able to collect data from appsettings.json
        private IConfiguration config;

        public IConfiguration Config
        {
            get { return config; }
            set { config = value; }
        }


        public JwtAuthenticationManager(IConfiguration config)
        {
            Config = config;
        }
        
        // this method is used to create a new JwtToken object with a generated token string based on various parameters
        public JwtToken GenerateToken(DateTime expiryDate, Users user)
        {
            return new JwtToken()
            {
                Username = user.Username,
                Token = GenerateJwtToken(expiryDate, user.Username),
            };
        }

        public JwtSecurityToken DecodeToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(token);

            return decodedToken;
        }

        public string GetDecodedTokenExpiryDate(string token)
        {
            var decodedToken = DecodeToken(token);

            var expDate = decodedToken.Claims.First(claim => claim.Type == "expiryDate").Value;

            return expDate;
        }

        // this method generates a unique JwtToken 
        private string GenerateJwtToken(DateTime expiryDate, string username)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            // get the hardCoded securityString from appsettings.json
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["Jwt:Key"]));
            // use HmacSha256 encryption
            SigningCredentials credentials =
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // create a JwtSecurityToken with various parameters
            // collect Issuer from appsettings.js
            // use specific expiryDate
            // set a variable name = username

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim("username:", username),
                new Claim("expiryDate", expiryDate.ToString())
            });


            var token = (JwtSecurityToken)
                tokenHandler.CreateJwtSecurityToken(issuer: Config["Jwt: Issuer"], audience: Config["Jwt: Issuer"],
                    claimsIdentity, expires: expiryDate, signingCredentials: credentials);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        // this method is used to refresh an existing token if the token is still valid
        
    }
}