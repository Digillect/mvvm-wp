using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Navigation;

namespace Digillect.Mvvm.UI
{
	/// <summary>
	/// Extensions to help using <see cref="System.Windows.Navigation.NavigationService"/> class.
	/// </summary>
	public static class NavigationServiceExtensions
	{
		/// <summary>
		/// Performs navigation to the page identified by code-behind class.
		/// </summary>
		/// <typeparam name="TPage">The type of the page's code-behind.</typeparam>
		/// <param name="navigationService">The navigation service.</param>
		/// <param name="queryString">The query string.</param>
		/// <exception cref="System.InvalidOperationException">when page namespace is not started with application type's namespace.</exception>
		/// <remarks>Main benefit of this method is that you can move your XAML pages along with code-behind classes without the need
		/// to changes paths everywhere in your application to reflect new page location. This methods relies on the fact that
		/// type, defining your application is located in root namespace of application assembly and that XAML files have the same name as the code-behind classes.</remarks>
		/// <example>Assuming that you have <c>YourAssemblyRootNamespace.Pages.AboutPage</c> class residing in <c>Pages</c> folder and
		/// <c>AboutPage.xaml</c> file residing in the same folder you can use following code to navigate to the page:
		/// <code>
		///		private void Button_Click( object sender, RoutedEventArgs e )
		///		{
		///			this.NavigationService.Navigate&lt;AboutPage&gt;();
		///		}
		/// </code>
		/// </example>
		public static void Navigate<TPage>( this NavigationService navigationService, string queryString = null )
			where TPage : PhoneApplicationPage
		{
			var pageType = typeof( TPage );
			var applicationType = Application.Current.GetType();

			if( !pageType.Namespace.StartsWith( applicationType.Namespace ) )
				throw new InvalidOperationException( "Page must be in the child namespace relative to Application's namespace." );

			var uri = string.Format( "{0}{1}/{2}.xaml", pageType.Namespace == applicationType.Namespace ? "/" : "", pageType.Namespace.Substring( applicationType.Namespace.Length ).Replace( '.', '/' ), pageType.Name );

			if( !string.IsNullOrWhiteSpace( queryString ) )
				uri += "?" + queryString;

			navigationService.Navigate( new Uri( uri, UriKind.Relative ) );
		}
	}
}
