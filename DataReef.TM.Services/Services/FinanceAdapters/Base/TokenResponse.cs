namespace DataReef.TM.Services.Services
{
    public class TokenResponse
    {
        public TokenResponse(string token)
        {
            Token = token;
        }

        public string Token { get; set; }
    }
}