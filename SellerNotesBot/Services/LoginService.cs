using SellerNotesBot.Model;
using System.Threading.Tasks;

namespace SellerNotesBot.Services
{
    public class LoginService
    {
        public async Task<LoginResult> LoginAsync(string userName, string password)
        {
            // Get the ID token based on username and password. Intentionally omitted.

            // Mock
            var loginResult = new LoginResult()
            {
                id_token = "token",
                success = true
            };
            return loginResult;
        }

        internal static string GenerateLoginUrl()
        {
            return "https://access.company.com/login";
        }
    }
}