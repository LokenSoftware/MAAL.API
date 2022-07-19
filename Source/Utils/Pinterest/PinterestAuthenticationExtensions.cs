using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace MAAL.API.Utils.Pinterest;

/// <summary> </summary>
internal static class PinterestAuthenticationExtensions
{
	/// <summary> </summary>
	[PublicAPI]
	internal static AuthenticationBuilder AddPinterest(this AuthenticationBuilder builder,
	                                                   Action<PinterestAuthenticationOptions> configuration)
	{
		builder.Services.TryAddSingleton<IPostConfigureOptions<PinterestAuthenticationOptions>, PinterestPostConfigureOptions>();
		return builder.AddOAuth<PinterestAuthenticationOptions, PinterestAuthenticationHandler>("Pinterest",
			"Pinterest",
			configuration);
	}
}