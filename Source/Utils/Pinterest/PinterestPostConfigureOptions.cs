using Microsoft.Extensions.Options;
using static MAAL.API.Utils.Pinterest.PinterestAuthenticationDefaults;

namespace MAAL.API.Utils.Pinterest;

/// <inheritdoc />
internal sealed class PinterestPostConfigureOptions : IPostConfigureOptions<PinterestAuthenticationOptions>
{
	/// <inheritdoc/>
	public void PostConfigure(string name, PinterestAuthenticationOptions options)
	{
		options.AuthorizationEndpoint = AuthorizationEndpoint;
		options.TokenEndpoint = TokenEndpoint;
		options.UserInformationEndpoint = UserInformationEndpoint;
	}
}