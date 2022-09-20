using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;
using MAAL.API.Utils;
using MAAL.API.Utils.MAAL;
using MAAL.API.Utils.Pinterest;
using MAAL.API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Raven.DependencyInjection;
using Raven.Identity;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// RavenDB
builder.Services.AddRavenDbDocStore().AddRavenDbAsyncSession();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity
builder.Services.AddIdentityCore<RavenUser>(options =>
	{
		options.SignIn.RequireConfirmedAccount = false;
		options.User.AllowedUserNameCharacters = null;
	})
	.AddRavenDbIdentityStores<RavenUser>()
	.AddSignInManager()
	.AddDefaultTokenProviders();

// Controllers
builder.Services.AddControllers();

// Authentication
AuthenticationBuilder authBuilder = builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = IdentityConstants.ApplicationScheme;
	options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
});
authBuilder.AddIdentityCookies();

// Authentication Providers
var providers = new Collection<LoginProvider>
{
	new(FacebookAuthenticationOptionsExtensions.AddFacebook, "Facebook"),
	new(GitHubAuthenticationExtensions.AddGitHub, "GitHub"),
	new(GoogleExtensions.AddGoogle, "Google"),
	new(MAALAuthenticationExtensions.AddMAAL, "MAAL"),
	new(MicrosoftAccountExtensions.AddMicrosoftAccount, "Microsoft"),
	new(PinterestAuthenticationExtensions.AddPinterest, "Pinterest"),
	new(TwitterAuthenticationExtensions.AddTwitter, "Twitter")
};
authBuilder.AddLoginProviders(providers, builder.Configuration);

// CORS
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policyBuilder =>
	{
		IConfigurationSection? cors = builder.Configuration.GetSection("CORS");
		string[]? allowedOrigins = cors?.GetSection("AllowedOrigins").Get<string[]>();
		if (cors == null || allowedOrigins == null)
		{
			throw new NullReferenceException("CORS__AllowedOrigins must be defined");
		}
		policyBuilder.WithOrigins(allowedOrigins);
		policyBuilder.WithHeaders("Content-Type");
		policyBuilder.WithMethods(HttpMethods.Get,
			HttpMethods.Post,
			HttpMethods.Patch,
			HttpMethods.Put,
			HttpMethods.Delete);
		policyBuilder.AllowCredentials();
	});
});

// Build app
WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
	app.UseHttpsRedirection();
}
else
{
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpLogging();

// We use Nginx
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
	ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseEndpoints(routeBuilder =>
{
	routeBuilder.MapGet("/Test",
		async context =>
		{
			Type[] types = Assembly.GetEntryAssembly()?.GetTypes() ?? Array.Empty<Type>();
			IReadOnlyCollection<Type> controllers = types
				.Where(type => typeof(Microsoft.AspNetCore.Mvc.ControllerBase).IsAssignableFrom(type))
				.ToList();

			IEnumerable<MethodInfo> methods = controllers
				.SelectMany(type =>
					type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
				.Where(info => !info
					.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true)
					.Any());

			IEnumerable<string> methodNames = methods.Select(info => info.Name);

			context.Response.StatusCode = StatusCodes.Status200OK;
			await context.Response.Body.WriteAsync(JsonSerializer.SerializeToUtf8Bytes(methodNames));
		});
});

app.Run();