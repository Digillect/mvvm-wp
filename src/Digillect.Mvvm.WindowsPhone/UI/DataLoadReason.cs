namespace Digillect.Mvvm.UI
{
	/// <summary>
	///     Specifies the reason to load page data.
	/// </summary>
	public enum DataLoadReason
	{
		/// <summary>
		///     Load data for the new instance of the page.
		/// </summary>
		New,

		/// <summary>
		///     Load data after page has been resurrected.
		/// </summary>
		Resurrection,

		/// <summary>
		///     Load data after restoring from dormant state.
		/// </summary>
		Awakening,

		/// <summary>
		///     Load data after the manual call to <see cref="ViewViewModelPage{TViewModel}.LoadData" />
		/// </summary>
		Manual,
	}
}