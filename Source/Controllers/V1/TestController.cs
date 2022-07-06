using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MAAL.API.Controllers.V1;

/// <inheritdoc />
[ApiController, Route("V1/[controller]"), Authorize]
public class TestController : ControllerBase
{
	/// <summary> </summary>
	/// <returns> </returns>
	[HttpGet]
	public string Get() => "hi";
}