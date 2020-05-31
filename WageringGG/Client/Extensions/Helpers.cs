using System.Linq;
using System.Security.Claims;

namespace WageringGG.Client
{
    public static class Helpers
    {
        public static string GetId(this ClaimsPrincipal User)
        {
            return User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
        }
    }
}
