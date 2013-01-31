using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;

using Digillect.Mvvm.UI;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	/// Autofac module that registers default and system services.
	/// </summary>
	[CLSCompliant( false )]
	public class WindowsPhoneModule : MvvmModule
	{
		/// <summary>
		/// Override to add registrations to the container.
		/// </summary>
		/// <param name="builder">The builder through which components can be
		/// registered.</param>
		/// <remarks>
		/// Note that the ContainerBuilder parameter is unique to this module.
		/// </remarks>
		protected override void Load( ContainerBuilder builder )
		{
			base.Load( builder );

			builder.RegisterType<NetworkAvailabilityService>().As<INetworkAvailabilityService,IStartable>().SingleInstance();
			builder.RegisterType<PageDecorationService>().As<IPageDecorationService>().SingleInstance();
			builder.RegisterType<NavigationService>().As<INavigationService, IStartable>().SingleInstance();

			builder.RegisterType<PageDataContext>().AsSelf();
			builder.RegisterType<ViewModelPageDataContext>().AsSelf();
		}
	}
}
