namespace PlanningPoker.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using PlanningPoker.Data;

public interface IAuthService
{

    public Task<UserState> GetSignedUserState();

}

public class AuthService : IAuthService
{

    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AuthService(AuthenticationStateProvider authenticationStateProvider) {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<UserState> GetSignedUserState() {
        var userState = new UserState();
        
        var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authenticationState.User;
        var claims = user.Claims.ToList();
        
        userState.UserName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty;
        string? userRole = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        userState.IsSpectator = userRole != "participant";
        
        return userState;
    }

}