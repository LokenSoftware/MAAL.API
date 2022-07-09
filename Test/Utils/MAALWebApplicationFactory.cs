using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MAAL.API.Test.Utils;

/// <inheritdoc />
[UsedImplicitly]
public sealed class MAALWebApplicationFactory : WebApplicationFactory<Program>
{
	/// <inheritdoc />
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureAppConfiguration((_, configurationBuilder) =>
		{
			configurationBuilder.AddInMemoryCollection(new[]
			{
				new KeyValuePair<string, string>("ConnectionStrings:Identity", "null"),
				new KeyValuePair<string, string>("Authentication:Google:ClientId", "null"),
				new KeyValuePair<string, string>("Authentication:Google:ClientSecret", "null")
			});
		});
	}
}