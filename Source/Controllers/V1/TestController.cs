using Microsoft.AspNetCore.Mvc;

namespace MAAL.API.Controllers.V1;

/// <inheritdoc />
[ApiController, Route("V1/[controller]")]
public class TestController : ControllerBase
{
	/// <summary> </summary>
	/// <returns> </returns>
	[HttpGet]
	public string Get() => "Hello from Vultr";

	/// <summary> </summary>
	/// <returns> </returns>
	[HttpGet("RedirectUri")]
	public string Get_RedirectUri() => Request.Scheme + Uri.SchemeDelimiter + Request.Host + Request.PathBase;
}