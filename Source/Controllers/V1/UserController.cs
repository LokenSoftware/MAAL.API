using MAAL.API.Bases;
using MAAL.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MAAL.API.Controllers.V1;

/// <inheritdoc />
[ApiController, Route("V1/[controller]"), Authorize]
public class UserController : MAALControllerBase
{
	/// <inheritdoc cref="UserManager{TUser}" />
	private readonly UserManager<RavenUser> _userManager;

	/// <inheritdoc />
	public UserController(ILogger<UserController> logger, UserManager<RavenUser> userManager) : base(logger) =>
		_userManager = userManager;

	/// <summary> Fetch information about logged in user </summary>
	[HttpGet]
	public async Task<MAALUser> Get()
	{
		try
		{
			RavenUser user = await _userManager.GetUserAsync(HttpContext.User);
			IList<UserLoginInfo>? login = await _userManager.GetLoginsAsync(user);
			string provider = login?.FirstOrDefault()?.ProviderDisplayName ?? "Unknown";
			return new MAALUser(user.Id!, user.UserName, user.Email, provider);
		}
		catch (Exception e)
		{
			HandleException(e);
			throw;
		}
	}

	/// <summary> Remove user entirely </summary>
	[HttpGet("Remove")]
	public async Task Get_Remove()
	{
		try
		{
			RavenUser user = await _userManager.GetUserAsync(HttpContext.User);
			await _userManager.DeleteAsync(user);
		}
		catch (Exception e)
		{
			HandleException(e);
			throw;
		}
	}
}