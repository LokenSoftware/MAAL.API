using System.Security.Claims;
using System.Web;
using MAAL.API.Bases;
using MAAL.API.Enums;
using MAAL.API.Models;
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
	/// <summary> Fully qualified base url to frontend </summary>
	private readonly string _frontendUrl;

	/// <summary> Fully qualified base url to self </summary>
	private readonly string _selfUrl;

	/// <inheritdoc cref="SignInManager{TUser}" />
	private readonly SignInManager<RavenUser> _signInManager;

	/// <inheritdoc cref="UserManager{TUser}" />
	private readonly UserManager<RavenUser> _userManager;

	/// <inheritdoc />
	public LoginController(ILogger<LoginController> logger,
	                       SignInManager<RavenUser> signInManager,
	                       UserManager<RavenUser> userManager,
	                       IConfiguration configuration) : base(logger)
	{
		_signInManager = signInManager;
		_userManager = userManager;
		_frontendUrl = configuration["FrontendUrl"];
		_selfUrl = configuration["SelfUrl"];
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
			var redirectUrl = $"{_selfUrl}/V1/Login/Callback?returnUrl={HttpUtility.UrlEncode(returnUrl)}";
			AuthenticationProperties properties
				= _signInManager.ConfigureExternalAuthenticationProperties(provider.ToString(), redirectUrl);

			properties.AllowRefresh = true;
			return Challenge(properties, provider.ToString());
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
			ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();

			var url = new Uri(new Uri(_frontendUrl), returnUrl);
			RedirectResult redirect = Redirect(url.ToString());
			SignInResult? result
				= await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);

			if (result.Succeeded)
			{
				return redirect;
			}
			if (result.IsLockedOut)
			{
				return Forbid();
			}

			string? email = info.Principal.FindFirstValue(ClaimTypes.Email);
			if (String.IsNullOrEmpty(email))
			{
				return BadRequest("Email was not found");
			}

			IdentityResult userResult;
			RavenUser? user = await _userManager.FindByEmailAsync(email);
			if (user != null)
			{
				userResult = await _userManager.UpdateAsync(user);
			}
			else
			{
				string username = info.Principal.Identity?.Name ?? email;
				user = new RavenUser
				{
					Email = email,
					UserName = username,
					Logins = { new UserLoginInfo(info.LoginProvider, info.ProviderKey, info.ProviderDisplayName) }
				};
				userResult = await _userManager.CreateAsync(user);
			}
			if (!userResult.Succeeded)
			{
				string errorString = String.Join(", ", userResult.Errors.Select(e => e.Description));
				return Problem(errorString);
			}

			await _signInManager.SignInAsync(user, false, info.LoginProvider);
			return redirect;
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
			await _signInManager.SignOutAsync();
		}
		catch (Exception e)
		{
			HandleException(e);
			throw;
		}
	}
}