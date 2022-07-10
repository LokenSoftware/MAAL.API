using System.Security.Claims;
using System.Web;
using MAAL.API.Bases;
using MAAL.API.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

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
			string redirectUrl = $"{Request.Scheme}://{Request.Host}/V1/Login/Callback?returnUrl={HttpUtility.UrlEncode(returnUrl)}";

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
			SignInResult? result = await _signInManager
				.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true, true)
				.ConfigureAwait(false);

			if (result.Succeeded)
			{
				return Ok();
			}
			if (result.IsLockedOut)
			{
				return Forbid();
			}

			var user = new IdentityUser();
			string? email = info.Principal.FindFirstValue(ClaimTypes.Email);
			await _userManager.SetUserNameAsync(user, email);
			await _userManager.SetEmailAsync(user, email);

			IdentityResult userResult = await _userManager.CreateAsync(user).ConfigureAwait(false);
			if (userResult.Succeeded)
			{
				await _signInManager.SignInAsync(user, false);
			}

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