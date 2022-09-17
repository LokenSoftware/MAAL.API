using System.Collections.ObjectModel;
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
		policyBuilder.WithMethods("GET", "POST", "PATCH", "PUT", "DELETE");
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
app.MapControllers();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.Run();