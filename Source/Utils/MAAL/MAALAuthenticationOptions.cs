using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace MAAL.API.Utils.MAAL;

/// <inheritdoc />
internal sealed class MAALAuthenticationOptions : OAuthOptions
{
	/// <inheritdoc />
	public MAALAuthenticationOptions()
	{
		ClaimsIssuer = MAALAuthenticationDefaults.Issuer;
		CallbackPath = MAALAuthenticationDefaults.CallbackPath;

		AuthorizationEndpoint = MAALAuthenticationDefaults.AuthorizationEndpoint;
		TokenEndpoint = MAALAuthenticationDefaults.TokenEndpoint;
		UserInformationEndpoint = MAALAuthenticationDefaults.UserInformationEndpoint;

		Scope.Add("profile");

		ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "Id");
		ClaimActions.MapJsonKey(ClaimTypes.Name, "Username");
	}
}