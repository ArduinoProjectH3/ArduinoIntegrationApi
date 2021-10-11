using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ArduinoIntegrationApi.DataModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ArduinoIntegrationApi.Authorization
{
    public class JwtAuthenticationManager
    {
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

        public JwtToken GenerateToken(DateTime expiryDate, Users user)
        {
            return new JwtToken()
            {
                Username = user.Username,
                Token = GenerateJwtToken(expiryDate, user.Username),
                ExpiryDate = expiryDate
            };
        }

        private string GenerateJwtToken(DateTime expiryDate, string username)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["Jwt:Key"]));
            SigningCredentials credentials =
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken token = new JwtSecurityToken(Config["Jwt: Issuer"],
                Config["Jwt:Issuer"],
                null,
                expires: expiryDate,
                signingCredentials: credentials);
            token.Payload["name"] = username;

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public void RefreshToken(JwtToken token)
        {
            DateTime expiryDate = DateTime.UtcNow.AddMinutes(30);
            token.Token = GenerateJwtToken(expiryDate, token.Username);
            token.ExpiryDate = expiryDate;
        }
    }
}