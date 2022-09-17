using Raven.Identity;

namespace MAAL.API.Models;

/// <inheritdoc />
public class RavenUser : IdentityUser
{
	public override string Email { get; set; }
}