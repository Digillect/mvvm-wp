#region Copyright (c) 2011-2014 Gregory Nickonov and Andrew Nefedkin (Actis® Wunderman)
// Copyright (c) 2011-2014 Gregory Nickonov and Andrew Nefedkin (Actis® Wunderman).
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
#endregion

using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

using Autofac;

using Microsoft.Phone.Net.NetworkInformation;

using NetworkInterface = Microsoft.Phone.Net.NetworkInformation.NetworkInterface;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	///     Default implementation of <see cref="Digillect.Mvvm.Services.INetworkAvailabilityService" /> for Windows Phone 7/8.
	/// </summary>
#if WINDOWS_PHONE_71
	public
#else
	internal
#endif
	sealed class NetworkAvailabilityService : INetworkAvailabilityService, IStartable
	{
		#region Constructors/Disposer
		#endregion

		#region Start
		/// <summary>
		///     Initializes a new instance of the <see cref="NetworkAvailabilityService" /> class.
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
		///     Gets a value indicating whether network connection is available.
		/// </summary>
		/// <value>
		///     <c>true</c> if network connection available; otherwise, <c>false</c>.
		/// </value>
		public bool NetworkAvailable { get; private set; }

		/// <summary>
		///     Occurs when network availability changed.
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
			bool oldNetworkAvailable = NetworkAvailable;

			NetworkAvailable = await TaskEx.Run( (Func<bool>) InspectNetwork );

			if( NetworkAvailable != oldNetworkAvailable && NetworkAvailabilityChanged != null )
			{
				NetworkAvailabilityChanged( this, EventArgs.Empty );
			}
		}

		private static bool InspectNetwork()
		{
			switch( NetworkInterface.NetworkInterfaceType )
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