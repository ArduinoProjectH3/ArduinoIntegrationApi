using Microsoft.Extensions.Configuration;

namespace ArduinoIntegrationApi.Authorization
{
    public class Authorizer
    {
        private IConfiguration Config { get; set; }
        private static string ApiKeyName => "apiKey";

        public Authorizer(IConfiguration config)
        {
            this.Config = config;
        }

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
