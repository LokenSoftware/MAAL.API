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
		
		// NOTE: NameIdentifier should be PK. Maybe username is PK in Pinterest's DB, but that would be unusual...
		ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "username");
		ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
	}
}