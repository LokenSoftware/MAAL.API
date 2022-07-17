using JetBrains.Annotations;

namespace MAAL.API.Enums;

/// <summary> Supported identity providers </summary>
[PublicAPI]
public enum IdentityProvider
{
	/// <summary> Facebook account </summary>
	Facebook,

	/// <summary> GitHub account </summary>
	GitHub,

	/// <summary> Google account </summary>
	Google,

	/// <summary> LinkedIn account </summary>
	LinkedIn,

	/// <summary> MAAL account </summary>
	MAAL,

	/// <summary> Microsoft account </summary>
	Microsoft,

	/// <summary> Pinterest account </summary>
	Pinterest,

	/// <summary> Twitter account </summary>
	Twitter
}