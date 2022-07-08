using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MAAL.API.Test.Utils;

/// <inheritdoc />
[UsedImplicitly]
public sealed class MAALWebApplicationFactory : WebApplicationFactory<Program> { }