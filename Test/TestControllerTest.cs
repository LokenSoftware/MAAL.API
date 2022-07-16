using MAAL.API.Test.Utils;
using Xunit;

namespace MAAL.API.Test;

/// <inheritdoc />
/// <summary> Test /V1/Test endpoints </summary>
public sealed class TestControllerTest : IClassFixture<MAALWebApplicationFactory<Program>>
{
	/// <summary> </summary>
	private readonly HttpClient _client;

	/// <summary> </summary>
	public TestControllerTest(MAALWebApplicationFactory<Program> factory) => _client = factory.CreateClient();

	/// <summary> </summary>
	[Fact]
	public async Task Should_Get()
	{
		HttpResponseMessage res = await _client.GetAsync("/V1/Test").ConfigureAwait(false);
		Assert.Equal(StatusCodes.Status200OK, (int)res.StatusCode);

		string str = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
		Assert.NotEmpty(str);
	}
}