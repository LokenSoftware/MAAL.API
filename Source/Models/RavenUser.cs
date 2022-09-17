using Raven.Identity;

namespace MAAL.API.Models;

/// <inheritdoc />
public class RavenUser : IdentityUser
{
	/// <inheritdoc />
	public override string Email { get; set; } = String.Empty;
}