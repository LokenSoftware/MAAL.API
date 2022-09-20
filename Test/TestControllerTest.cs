using System.Net;
using MAAL.API.Test.Utils;
using Xunit;

namespace MAAL.API.Test;

/// <summary> Test /V1/Test endpoints </summary>
public sealed class TestControllerTest
{
	/// <summary> </summary>
	private readonly HttpClient _client;

	/// <summary> xUnit constructs this on every test function, so we don't need to repeat CreateClient </summary>
	public TestControllerTest() => _client = new MAALWebApplicationFactory<Program>().CreateClient();

	/// <summary> </summary>
	[Fact]
	public async Task Get_Ping_ResultsIn200OK()
	{
		HttpResponseMessage res = await _client.GetAsync("/Ping");
		Assert.Equal(HttpStatusCode.OK, res.StatusCode);
	}
}