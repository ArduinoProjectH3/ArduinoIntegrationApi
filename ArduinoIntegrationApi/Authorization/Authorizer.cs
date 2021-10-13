using Microsoft.Extensions.Configuration;

namespace ArduinoIntegrationApi.Authorization
{
    /// <summary>
    /// Authorizer class contains logic to authorize api key
    /// </summary>
    public class Authorizer
    {
        // property to access Config
        private IConfiguration Config { get; set; }
        // private variable to hold the apiKey value
        private static string ApiKeyName => "apiKey";

        public Authorizer(IConfiguration config)
        {
            this.Config = config;
        }

        // method to validate if the users apiKey is equal to the api's apiKey from appsettings.json
        public bool ClientIsAuthorized(string apiKeyFromClient)
        {
            bool clientIsAuthorized = false;

            string apiKeyFromWebServer = Config.GetValue<string>(ApiKeyName);

            if (apiKeyFromWebServer == apiKeyFromClient)
            {
                clientIsAuthorized = true;
            }

            return clientIsAuthorized;
        }
    }
}
