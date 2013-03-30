#region Copyright (c) 2011-2013 Gregory Nickonov and Andrew Nefedkin (Actis® Wunderman)
// Copyright (c) 2011-2013 Gregory Nickonov and Andrew Nefedkin (Actis® Wunderman).
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

using Autofac;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	///     Autofac module that registers default and system services.
	/// </summary>
	[CLSCompliant( false )]
	public class WindowsPhoneModule : MvvmModule
	{
		/// <summary>
		///     Override to add registrations to the container.
		/// </summary>
		/// <param name="builder">
		///     The builder through which components can be
		///     registered.
		/// </param>
		/// <remarks>
		///     Note that the ContainerBuilder parameter is unique to this module.
		/// </remarks>
		protected override void Load( ContainerBuilder builder )
		{
			base.Load( builder );

			builder.RegisterType<NetworkAvailabilityService>().As<INetworkAvailabilityService, IStartable>().SingleInstance();
			builder.RegisterType<PageDecorationService>().As<IPageDecorationService>().SingleInstance();
			builder.RegisterType<NavigationService>().As<INavigationService, IWindowsPhoneNavigationService, IStartable>().SingleInstance();

			builder.RegisterType<AuthenticationService>()
					.As<IAuthenticationService, INavigationHandler, IStartable>()
					.SingleInstance()
					.OnActivated( e => e.Instance.SetNavigationService( e.Context.Resolve<IWindowsPhoneNavigationService>() ) );
		}
	}
}