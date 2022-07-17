using JetBrains.Annotations;

namespace MAAL.API.Models;

/// <summary> Normalized user information to send to frontend </summary>
/// <param name="Id"> Unique identifier for user in Identity system </param>
/// <param name="Name"> </param>
/// <param name="Email"> </param>
[UsedImplicitly]
public sealed record MAALUser(string Id, string Name, string Email, string Provider);