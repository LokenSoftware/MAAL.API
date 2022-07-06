using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MAAL.API.Data;

/// <inheritdoc />
public class ApplicationDbContext : IdentityDbContext
{
	/// <inheritdoc />
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}