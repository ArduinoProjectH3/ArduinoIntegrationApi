namespace ArduinoIntegrationApi.Interfaces
{
    public interface IJwtAuthenticationManager
    {
        string Authenticate(string username, string password);

    }
}
