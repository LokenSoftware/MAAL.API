using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Options;

namespace MAAL.API.Utils.Pinterest;

/// <inheritdoc />
internal sealed class PinterestAuthenticationHandler : OAuthHandler<PinterestAuthenticationOptions>
{
	/// <inheritdoc />
	public PinterestAuthenticationHandler(
			IOptionsMonitor<PinterestAuthenticationOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder,
			ISystemClock clock
		) : base(options, logger, encoder, clock) { }

	/// <inheritdoc />
	protected override async Task<AuthenticationTicket> CreateTicketAsync(
		ClaimsIdentity identity,
		AuthenticationProperties properties,
		OAuthTokenResponse tokens)
	{
		using var request = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint);
		request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

		using HttpResponseMessage response = await Backchannel.SendAsync(request,
			HttpCompletionOption.ResponseHeadersRead,
			Context.RequestAborted);

		if (!response.IsSuccessStatusCode)
		{
			await Log.UserProfileErrorAsync(Logger, response, Context.RequestAborted);
			throw new HttpRequestException("An error occurred while retrieving the user profile.");
		}

		using JsonDocument payload
			= JsonDocument.Parse(await response.Content.ReadAsStringAsync(Context.RequestAborted));

		var principal = new ClaimsPrincipal(identity);
		var context = new OAuthCreatingTicketContext(principal,
			properties,
			Context,
			Scheme,
			Options,
			Backchannel,
			tokens,
			payload.RootElement);

		context.RunClaimActions();
		await Events.CreatingTicket(context);
		return new AuthenticationTicket(context.Principal!, context.Properties, Scheme.Name);
	}

	/// <summary> </summary>
	private static class Log
	{
		/// <summary> </summary>
		internal static async Task UserProfileErrorAsync(
				ILogger logger,
				HttpResponseMessage response,
				CancellationToken cancellationToken)
		{
			string body = await response.Content.ReadAsStringAsync(cancellationToken);
			logger.LogError(
				"An error occurred while retrieving the user profile: the remote server returned a {Status} response with the following payload: {Headers} {Body}",
				response.StatusCode,
				response.Headers.ToString(),
				body);
		}
	}
}