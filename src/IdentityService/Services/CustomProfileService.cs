using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
namespace IdentityService.Services;
public class CustomProfileService(UserManager<ApplicationUser> userManager) : IProfileService
{
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await userManager.GetUserAsync(context.Subject);
        if (user == null || user.UserName == null) return;
        var existingClaims = await userManager.GetClaimsAsync(user);
        var nameClaim = existingClaims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name);
        var roleClaim = existingClaims.FirstOrDefault(x => x.Type == JwtClaimTypes.Role);
        var claims = new List<Claim>
        {
            new Claim("username", user.UserName)
        };
        context.IssuedClaims.AddRange(claims);
        
        if (nameClaim != null)
            context.IssuedClaims.Add(nameClaim);

        if (roleClaim != null)
            context.IssuedClaims.Add(roleClaim);
    }
    public Task IsActiveAsync(IsActiveContext context)
    {
        return Task.CompletedTask;
    }
}
