using System;
using System.Diagnostics.Contracts;

namespace Digillect.Mvvm.UI
{
	/// <summary>
	/// Specifies the path to the attributed view.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
	public sealed class ViewPathAttribute : Attribute
	{
		private readonly string _path;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewPathAttribute"/> class.
		/// </summary>
		/// <param name="path">The path to the view.</param>
		/// <exception cref="ArgumentNullException">If <paramref name="path"/> is <c>null</c>.</exception>
		public ViewPathAttribute( string path )
		{
			if( path == null )
			{
				throw new ArgumentNullException( "path" );
			}

			Contract.EndContractBlock();

			_path = path;
		}

		/// <summary>
		/// Gets the path to the view.
		/// </summary>
		public string Path
		{
			get { return _path; }
		}
	}
}
