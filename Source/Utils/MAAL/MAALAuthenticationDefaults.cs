namespace MAAL.API.Utils.MAAL;

/// <summary> </summary>
internal static class MAALAuthenticationDefaults
{
	/// <summary> </summary>
	public const string Issuer = "MAAL";

	/// <summary> </summary>
	public const string CallbackPath = "/signin-maal";

	/// <summary> </summary>
	public const string AuthorizationEndpoint = "https://login.maal.dev/V1/auth";

	/// <summary> </summary>
	public const string TokenEndpoint = "https://login.maal.dev/V1/token";

	/// <summary> </summary>
	public const string UserInformationEndpoint = "https://login.maal.dev/V1/user";
}