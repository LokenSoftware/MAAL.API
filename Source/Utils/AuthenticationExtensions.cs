using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace MAAL.API.Utils;

/// <summary> </summary>
internal static class AuthenticationExtensions
{
	/// <summary> </summary>
	public static AuthenticationBuilder AddLoginProviders(this AuthenticationBuilder builder,
	                                                      IEnumerable<LoginProvider> loginProviders,
	                                                      ConfigurationManager configuration)
	{
		IConfigurationSection authentication = configuration.GetSection("Authentication");
		foreach (LoginProvider provider in loginProviders)
		{
			provider.AddFunction(builder,
				options =>
				{
					IConfigurationSection google = authentication.GetSection(provider.Key);
					string? clientId = google["ClientId"];
					string? clientSecret = google["ClientSecret"];
					if (clientId == null || clientSecret == null)
					{
						throw new NullReferenceException(
							$"Authentication__{provider.Key}__ClientId and ClientSecret must be defined");
					}
					options.ClientId = clientId;
					options.ClientSecret = clientSecret;
				});
		}
		return builder;
	}
}

/// <summary> </summary>
internal sealed record LoginProvider(
	Func<AuthenticationBuilder, Action<OAuthOptions>, AuthenticationBuilder> AddFunction,
	string Key);