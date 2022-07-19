using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace MAAL.API.Utils.Pinterest;

/// <inheritdoc />
internal sealed class PinterestAuthenticationOptions : OAuthOptions
{
	/// <inheritdoc />
	public PinterestAuthenticationOptions()
	{
		ClaimsIssuer = PinterestAuthenticationDefaults.Issuer;
		CallbackPath = PinterestAuthenticationDefaults.CallbackPath;

		AuthorizationEndpoint = PinterestAuthenticationDefaults.AuthorizationEndpoint;
		TokenEndpoint = PinterestAuthenticationDefaults.TokenEndpoint;
		UserInformationEndpoint = PinterestAuthenticationDefaults.UserInformationEndpoint;

		Scope.Add("user_accounts:read");
		
		ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "username");
		ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
	}
}