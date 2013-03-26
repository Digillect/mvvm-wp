using System;

namespace Digillect.Mvvm.UI
{
	/// <summary>
	/// Specifies that attributed view is part of the authentication flow.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
	public sealed class ViewIsPartOfAuthenticationFlowAttribute : Attribute
	{
	}
}