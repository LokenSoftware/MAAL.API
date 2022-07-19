namespace MAAL.API.Utils.Pinterest;

/// <summary> </summary>
internal static class PinterestAuthenticationDefaults
{
	/// <summary> </summary>
	public const string Issuer = "Pinterest";

	/// <summary> </summary>
	public const string CallbackPath = "/signin-pinterest";

	/// <summary> </summary>
	public const string AuthorizationEndpoint = "https://www.pinterest.com/oauth";

	/// <summary> </summary>
	public const string TokenEndpoint = "https://api.pinterest.com/v5/oauth/token";

	/// <summary> </summary>
	public const string UserInformationEndpoint = "https://api.pinterest.com/v5/user_account";
}