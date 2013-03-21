using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Retail")]
#endif
[assembly: AssemblyCompany( "Actis Systems" )]
[assembly: AssemblyProduct("Windows Phone adaptor for Digillect� Model-View-ViewModel framework.")]
[assembly: AssemblyCopyright("� 2011-2012 Actis Systems. All rights reserved.")]
[assembly: AssemblyTrademark("Digillect is a registered trademark of Actis Systems.")]

[assembly: AssemblyVersion(AssemblyInfo.Version)]
[assembly: AssemblyFileVersion(AssemblyInfo.FileVersion)]
[assembly: AssemblyInformationalVersion(AssemblyInfo.ProductVersion)]

[assembly: CLSCompliant(false)]

[assembly: ComVisible(false)]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: SatelliteContractVersion(AssemblyInfo.SatelliteContractVersion)]

internal static class AssemblyInfo
{
	public const string Major = "2";
	public const string Minor = "0";
	public const string Revision = "0";
	public const string BuildNumber = "0";

	public const string Version = Major + "." + Minor + "." + Revision + ".0";
	public const string FileVersion = Major + "." + Minor + "." + Revision + "." + BuildNumber;
	public const string ProductVersion = Major + "." + Minor + "." + Revision;
	public const string SatelliteContractVersion = Major + "." + Minor + ".0.0";
}
