using System.Security.Claims;

namespace CartService.IntegrationTests.Utils;

public class AuthHelper
{
    public static Dictionary<string, object> GetBearerForUser(string username)
    {
        return new Dictionary<string, object>{{ClaimTypes.Name, username}};
    }

    public static Dictionary<string, object> GetBearerForRole(string role)
    {
        return new Dictionary<string, object>{{ClaimTypes.Role, role}};
    }
}
