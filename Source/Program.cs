using JetBrains.Annotations;
using MAAL.API.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// EF Core Identity
string? connectionString = builder.Configuration.GetConnectionString("Identity");
if (connectionString == null)
{
	// TODO: TESTS do not work with this currently
	//throw new NullReferenceException("ConnectionStrings__Identity must be defined");
	Console.WriteLine("ConnectionStrings__Identity was null");
}
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseMySql(connectionString!, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity
builder.Services.AddIdentityCore<IdentityUser>(options =>
	{
		options.SignIn.RequireConfirmedAccount = false;
		options.User.AllowedUserNameCharacters = null;
	})
	.AddEntityFrameworkStores<ApplicationDbContext>()
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
IConfigurationSection authentication = builder.Configuration.GetSection("Authentication");

// Google
authBuilder.AddGoogle(options =>
{
	IConfigurationSection google = authentication.GetSection("Google");
	string? clientId = google["ClientId"];
	string? clientSecret = google["ClientSecret"];
	if (clientId == null || clientSecret == null)
	{
		throw new NullReferenceException("Authentication__Google__ClientId and ClientSecret must be defined");
	}
	options.ClientId = clientId;
	options.ClientSecret = clientSecret;
});

// Twitter
authBuilder.AddTwitter(options =>
{
	IConfigurationSection? twitter = authentication.GetSection("Twitter");
	string? clientId = twitter["ClientId"];
	string? clientSecret = twitter["ClientSecret"];
	if (clientId == null || clientSecret == null)
	{
		throw new NullReferenceException("Authentication__Twitter__ClientId and ClientSecret must be defined");
	}
	options.ClientId = clientId;
	options.ClientSecret = clientSecret;
});

// GitHub
authBuilder.AddGitHub(options =>
{
	IConfigurationSection? gitHub = authentication.GetSection("GitHub");
	string? clientId = gitHub["ClientId"];
	string? clientSecret = gitHub["ClientSecret"];
	if (clientId == null || clientSecret == null)
	{
		throw new NullReferenceException("Authentication__GitHub__ClientId and ClientSecret must be defined");
	}
	options.ClientId = clientId;
	options.ClientSecret = clientSecret;
});

// Microsoft
authBuilder.AddMicrosoftAccount(options =>
{
	IConfigurationSection? microsoft = authentication.GetSection("Microsoft");
	string? clientId = microsoft["ClientId"];
	string? clientSecret = microsoft["ClientSecret"];
	if (clientId == null || clientSecret == null)
	{
		throw new NullReferenceException("Authentication__Microsoft__ClientId and ClientSecret must be defined");
	}
	options.ClientId = clientId;
	options.ClientSecret = clientSecret;
});

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
		policyBuilder.WithMethods("OPTIONS", "GET", "POST", "PATCH", "PUT", "DELETE");
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

/// <summary> So tests can reference, but this shouldn't be necessary... </summary>
[UsedImplicitly]
#pragma warning disable CA1050
public partial class Program { }
#pragma warning restore CA1050