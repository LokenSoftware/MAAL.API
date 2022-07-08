using Microsoft.AspNetCore.Mvc;

namespace MAAL.API.Controllers;

/// <inheritdoc />
[ApiController, Route("[controller]")]
public class PingController : ControllerBase
{
	/// <summary> </summary>
	private readonly ILogger<PingController> _logger;

	/// <summary> </summary>
	public PingController(ILogger<PingController> logger) => _logger = logger;

	/// <summary> </summary>
	[HttpGet]
	public void Get() => _logger.LogInformation("OK");

	/// <summary> </summary>
	[HttpGet("Critical")]
	public void Get_Critical() => _logger.LogCritical("Critical");
}