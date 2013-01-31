using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

using Microsoft.Phone.Net.NetworkInformation;

using Autofac;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	/// Default implementation of <see cref="Digillect.Mvvm.Services.INetworkAvailabilityService"/> for Windows Phone 7/8.
	/// </summary>
	public sealed class NetworkAvailabilityService : INetworkAvailabilityService, IStartable
	{
		#region Constructors/Disposer
		/// <summary>
		/// Initializes a new instance of the NetworkAvailabilityService class.
		/// </summary>
		public NetworkAvailabilityService()
		{
		}
		#endregion

		#region Start
		/// <summary>
		/// Initializes a new instance of the <see cref="NetworkAvailabilityService"/> class.
		/// </summary>
		public void Start()
		{
			NetworkAvailable = true;

#if !WINDOWS_PHONE_8
			NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;

			NetworkChange_NetworkAddressChanged( null, null );
#else
			DeviceNetworkInformation.NetworkAvailabilityChanged += DeviceNetworkInformation_NetworkAvailabilityChanged;

			DeviceNetworkInformation_NetworkAvailabilityChanged( null, null );
#endif
		}
		#endregion

		/// <summary>
		/// Gets a value indicating whether network connection is available.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if network connection available; otherwise, <c>false</c>.
		/// </value>
		public bool NetworkAvailable { get; private set; }
		/// <summary>
		/// Occurs when network availability changed.
		/// </summary>
		public event EventHandler NetworkAvailabilityChanged;

#if WINDOWS_PHONE_8
		void DeviceNetworkInformation_NetworkAvailabilityChanged( object sender, NetworkNotificationEventArgs e )
		{
			var oldNetworkAvailable = NetworkAvailable;

			NetworkAvailable = DeviceNetworkInformation.IsNetworkAvailable;

			if( NetworkAvailable != oldNetworkAvailable && NetworkAvailabilityChanged != null )
			{
				NetworkAvailabilityChanged( this, EventArgs.Empty );
			}
		}
#endif

#if !WINDOWS_PHONE_8
		private async void NetworkChange_NetworkAddressChanged( object sender, EventArgs e )
		{
			var oldNetworkAvailable = NetworkAvailable;

			NetworkAvailable = await TaskEx.Run<bool>( (Func<bool>) InspectNetwork );

			if( NetworkAvailable != oldNetworkAvailable && NetworkAvailabilityChanged != null )
			{
				NetworkAvailabilityChanged( this, EventArgs.Empty );
			}
		}

		private static bool InspectNetwork()
		{
			switch( Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType )
			{
				case NetworkInterfaceType.Wireless80211:
				case NetworkInterfaceType.MobileBroadbandGsm:
				case NetworkInterfaceType.MobileBroadbandCdma:
				case NetworkInterfaceType.Ethernet:
					return true;

				default:
					break;
			}

			return false;
		}
#endif
	}
}
