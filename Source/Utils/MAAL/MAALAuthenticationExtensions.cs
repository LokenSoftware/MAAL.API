using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace MAAL.API.Utils.MAAL;

/// <summary> </summary>
internal static class MAALAuthenticationExtensions
{
	/// <summary> </summary>
	[PublicAPI]
	internal static AuthenticationBuilder AddMAAL(this AuthenticationBuilder builder,
	                                              Action<MAALAuthenticationOptions> configuration)
	{
		builder.Services.TryAddSingleton<IPostConfigureOptions<MAALAuthenticationOptions>, MAALPostConfigureOptions>();
		return builder.AddOAuth<MAALAuthenticationOptions, MAALAuthenticationHandler>("MAAL", "MAAL", configuration);
	}
}