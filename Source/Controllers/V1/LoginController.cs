using System.Web;
using MAAL.API.Bases;
using MAAL.API.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MAAL.API.Controllers.V1;

/// <inheritdoc />
[ApiController, Route("V1/[controller]")]
public class LoginController : MAALControllerBase
{
	/// <inheritdoc cref="SignInManager{TUser}" />
	private readonly SignInManager<IdentityUser> _signInManager;

	/// <inheritdoc cref="UserManager{TUser}" />
	private readonly UserManager<IdentityUser> _userManager;

	/// <summary> </summary>
	private readonly string _frontendUrl;

	/// <inheritdoc />
	public LoginController(ILogger<LoginController> logger,
	                       SignInManager<IdentityUser> signInManager,
	                       UserManager<IdentityUser> userManager,
	                       IConfiguration configuration) : base(logger)
	{
		_signInManager = signInManager;
		_userManager = userManager;
		_frontendUrl = configuration["FrontendUrl"];
	}

	/// <summary> Login with specific provider </summary>
	/// <param name="provider"> </param>
	/// <param name="returnUrl"> Url to return to after login </param>
	/// <returns> Challenge for login </returns>
	[HttpPost]
	public IActionResult Post([FromForm] IdentityProvider provider, [FromQuery] string returnUrl)
	{
		try
		{
			string redirectUrl = $"{Request.Host}/V1/Login/Callback?returnUrl={HttpUtility.UrlEncode(returnUrl)}";

			string providerName = provider switch
			{
				IdentityProvider.Microsoft => "Microsoft",
				IdentityProvider.Google => "Google",
				IdentityProvider.Twitter => "Twitter",
				IdentityProvider.LinkedIn => "LinkedIn",
				IdentityProvider.MAAL => "MAAL",
				_ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
			};

			AuthenticationProperties properties
				= _signInManager.ConfigureExternalAuthenticationProperties(providerName, redirectUrl);

			properties.AllowRefresh = true;
			return Challenge(properties, providerName);
		}
		catch (Exception e)
		{
			HandleException(e);
			throw;
		}
	}

	/// <summary> Sign in user to backend's identity system </summary>
	/// <returns> Redirect to return url specified in login </returns>
	[HttpGet("Callback")]
	public async Task<IActionResult> Get_Callback([FromQuery] string returnUrl)
	{
		try
		{
			ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync().ConfigureAwait(false);
			IdentityUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey)
				.ConfigureAwait(false);

			await _signInManager.SignInAsync(user, true).ConfigureAwait(false);

			// var options = new CookieOptions
			// {
			// 	//Needed so that domain.com can access  the cookie set by api.domain.com
			// 	Domain = "localhost", Expires = DateTime.UtcNow.AddMinutes(5)
			// };

			var url = new Uri(new Uri(_frontendUrl), returnUrl);
			return Redirect(url.ToString());
		}
		catch (Exception e)
		{
			HandleException(e);
			throw;
		}
	}

	/// <summary> Log out user </summary>
	[HttpPost("Logout"), Authorize]
	public async Task Post_Logout()
	{
		try
		{
			await _signInManager.SignOutAsync().ConfigureAwait(false);
		}
		catch (Exception e)
		{
			HandleException(e);
			throw;
		}
	}
}