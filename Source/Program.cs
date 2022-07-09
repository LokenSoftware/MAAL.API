using JetBrains.Annotations;
using MAAL.API.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// EF Core Identity
string? connectionString = builder.Configuration.GetConnectionString("Identity");
if (connectionString == null)
{
	//throw new NullReferenceException("ConnectionStrings__Identity must be defined");
}
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseMySql(connectionString!, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity
builder.Services.AddIdentityCore<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
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
}
else
{
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();

app.Run();

/// <summary> So tests can reference, but this shouldn't be necessary... </summary>
[UsedImplicitly]
#pragma warning disable CA1050
public partial class Program { }
#pragma warning restore CA1050