namespace Digillect.Mvvm.Services
{
	/// <summary>
	/// Context that describes navigation operation.
	/// </summary>
	public class NavigationContext
	{
		/// <summary>
		/// Gets or sets the name of the view.
		/// </summary>
		/// <value>
		/// The name of the view.
		/// </value>
		public string ViewName { get; set; }
		/// <summary>
		/// Gets or sets view parameters.
		/// </summary>
		/// <value>
		/// The parameters.
		/// </value>
		public Parameters Parameters { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether the navigation operation with this context should be cancelled.
		/// </summary>
		/// <value>
		///   <c>true</c> to cancel navigation; otherwise, <c>false</c>.
		/// </value>
		public bool Cancel { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether the currently presented view should be removed from navigation journal.
		/// </summary>
		/// <value>
		///   <c>true</c> to displace current view; otherwise, <c>false</c>.
		/// </value>
		public bool DisplaceCurrentView { get; set; }
	}
}