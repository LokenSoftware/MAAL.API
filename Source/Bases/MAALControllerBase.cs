using Microsoft.AspNetCore.Mvc;

namespace MAAL.API.Bases;

/// <inheritdoc />
public abstract class MAALControllerBase : ControllerBase
{
	/// <summary> Logger for controller </summary>
	private readonly ILogger<MAALControllerBase> _logger;

	/// <summary> </summary>
	/// <param name="logger"> </param>
	protected MAALControllerBase(ILogger<MAALControllerBase> logger) => _logger = logger;

	/// <summary> Log information about endpoint method, path and stack trace, as well as set status code to 500 </summary>
	/// <param name="exception"> Pipe in exception from catch clause </param>
	/// <param name="statusCode"> Defaults to 500 </param>
	protected void HandleException(Exception exception, int statusCode = StatusCodes.Status500InternalServerError)
	{
		Response.StatusCode = statusCode;
		_logger.LogError(exception,
			"{Method}: {Path}{Query}",
			Request.Method,
			Request.Path,
			Request.QueryString.ToString());
	}
}
