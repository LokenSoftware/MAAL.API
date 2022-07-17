using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace MAAL.API.Test.Utils;

/// <inheritdoc />
[UsedImplicitly]
public sealed class MAALWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
	/// <inheritdoc />
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		IConfigurationRoot? config = new ConfigurationBuilder().AddInMemoryCollection(new[]
			{
				new KeyValuePair<string, string>("ConnectionStrings:Identity", "null"),
				new KeyValuePair<string, string>("Authentication:Google:ClientId", "null"),
				new KeyValuePair<string, string>("Authentication:Google:ClientSecret", "null"),
				new KeyValuePair<string, string>("Authentication:Twitter:ClientId", "null"),
				new KeyValuePair<string, string>("Authentication:Twitter:ClientSecret", "null"),
				new KeyValuePair<string, string>("Authentication:GitHub:ClientId", "null"),
				new KeyValuePair<string, string>("Authentication:GitHub:ClientSecret", "null"),
				new KeyValuePair<string, string>("Authentication:Microsoft:ClientId", "null"),
				new KeyValuePair<string, string>("Authentication:Microsoft:ClientSecret", "null"),
				new KeyValuePair<string, string>("Authentication:LinkedIn:ClientId", "null"),
				new KeyValuePair<string, string>("Authentication:LinkedIn:ClientSecret", "null"),
				new KeyValuePair<string, string>("Authentication:MAAL:ClientId", "null"),
				new KeyValuePair<string, string>("Authentication:MAAL:ClientSecret", "null"),
				new KeyValuePair<string, string>("Authentication:Pinterest:ClientId", "null"),
				new KeyValuePair<string, string>("Authentication:Pinterest:ClientSecret", "null"),
				new KeyValuePair<string, string>("Authentication:Facebook:ClientId", "null"),
				new KeyValuePair<string, string>("Authentication:Facebook:ClientSecret", "null")
			})
			.Build();

		builder.UseConfiguration(config);
		builder.ConfigureTestServices(services => services.AddSingleton<IPolicyEvaluator, TestPolicyEvaluator>());
	}
}