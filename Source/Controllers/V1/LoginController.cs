﻿using System.Security.Claims;
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
	/// <summary> Fully qualified base url to frontend </summary>
	private readonly string _frontendUrl;

	/// <summary> Fully qualified base url to self </summary>
	private readonly string _selfUrl;

	/// <inheritdoc cref="SignInManager{TUser}" />
	private readonly SignInManager<IdentityUser> _signInManager;

	/// <inheritdoc cref="UserManager{TUser}" />
	private readonly UserManager<IdentityUser> _userManager;

	/// <inheritdoc />
	public LoginController(ILogger<LoginController> logger,
	                       SignInManager<IdentityUser> signInManager,
	                       UserManager<IdentityUser> userManager,
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
			ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync().ConfigureAwait(false);

			var url = new Uri(new Uri(_frontendUrl), returnUrl);
			RedirectResult redirect = Redirect(url.ToString());
			SignInResult? result = await _signInManager
				.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true)
				.ConfigureAwait(false);

			if (result.Succeeded)
			{
				return redirect;
			}
			if (result.IsLockedOut)
			{
				return Forbid();
			}

			var user = new IdentityUser();
			string? email = info.Principal.FindFirstValue(ClaimTypes.Email);
			await _userManager.SetEmailAsync(user, email);

			string? username = info.Principal.Identity?.Name ?? email;
			await _userManager.SetUserNameAsync(user, username);

			IdentityResult userResult = await _userManager.CreateAsync(user).ConfigureAwait(false);
			if (!userResult.Succeeded)
			{
				return Problem("User could not be created");
			}

			await _signInManager.SignInAsync(user, false, info.LoginProvider);

			// Should be moved to email confirmation: https://github.com/dotnet/aspnetcore/blob/main/src/Identity/UI/src/Areas/Identity/Pages/V5/Account/ExternalLogin.cshtml.cs
			await _userManager.AddLoginAsync(user, info).ConfigureAwait(false);
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
			await _signInManager.SignOutAsync().ConfigureAwait(false);
		}
		catch (Exception e)
		{
			HandleException(e);
			throw;
		}
	}
}