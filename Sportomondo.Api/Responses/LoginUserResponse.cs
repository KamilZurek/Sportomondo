namespace Sportomondo.Api.Responses
{
    public class LoginUserResponse
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
