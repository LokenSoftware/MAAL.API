using Microsoft.Extensions.Options;
using static MAAL.API.Utils.MAAL.MAALAuthenticationDefaults;

namespace MAAL.API.Utils.MAAL;

/// <inheritdoc />
internal sealed class MAALPostConfigureOptions : IPostConfigureOptions<MAALAuthenticationOptions>
{
	/// <inheritdoc />
	public void PostConfigure(string name, MAALAuthenticationOptions options)
	{
		options.AuthorizationEndpoint = AuthorizationEndpoint;
		options.TokenEndpoint = TokenEndpoint;
		options.UserInformationEndpoint = UserInformationEndpoint;
	}
}