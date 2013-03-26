using System;

namespace Digillect.Mvvm.UI
{
	/// <summary>
	/// Specifies that attributed view requires authentication to run.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
	public sealed class ViewRequiresAuthenticationAttribute : Attribute
	{
	}
}