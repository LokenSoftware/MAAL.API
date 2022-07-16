using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Identity;

namespace MAAL.API.Test.Utils;

/// <inheritdoc />
public sealed class TestPolicyEvaluator : IPolicyEvaluator
{
	/// <inheritdoc />
	public Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
	{
		ClaimsIdentity claims = new(IdentityConstants.ApplicationScheme);

		// Modify claims to test specific access.
		// Note that these are meaningless in terms of authorization if AuthorizeAsync returns Success regardless
		claims.AddClaim(new Claim(ClaimTypes.Name, @"mathiasloeken@outlook.com"));

		ClaimsPrincipal principal = new(claims);
		AuthenticationTicket ticket = new(principal,
			new AuthenticationProperties(),
			IdentityConstants.ApplicationScheme);

		return Task.FromResult(AuthenticateResult.Success(ticket));
	}

	/// <inheritdoc />
	public Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy,
	                                                      AuthenticateResult authenticationResult,
	                                                      HttpContext context,
	                                                      object? resource) =>
		Task.FromResult(PolicyAuthorizationResult.Success());
}