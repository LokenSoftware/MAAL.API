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
	public async Task Should_Get()
	{
		HttpResponseMessage res = await _client.GetAsync("/Ping").ConfigureAwait(false);
		Assert.Equal(StatusCodes.Status200OK, (int)res.StatusCode);
	}
}