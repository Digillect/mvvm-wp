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
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

#if DEBUG

[assembly: AssemblyConfiguration( "Debug" )]
#else
[assembly: AssemblyConfiguration("Retail")]
#endif

[assembly: AssemblyCompany( "Actis Systems" )]
[assembly: AssemblyProduct( "Windows Phone adaptor for Digillect� Model-View-ViewModel framework." )]
[assembly: AssemblyCopyright( "� 2011-2012 Actis Systems. All rights reserved." )]
[assembly: AssemblyTrademark( "Digillect is a registered trademark of Actis Systems." )]
[assembly: AssemblyVersion( AssemblyInfo.Version )]
[assembly: AssemblyFileVersion( AssemblyInfo.FileVersion )]
[assembly: AssemblyInformationalVersion( AssemblyInfo.ProductVersion )]
[assembly: CLSCompliant( false )]
[assembly: ComVisible( false )]
[assembly: NeutralResourcesLanguage( "en-US" )]
[assembly: SatelliteContractVersion( AssemblyInfo.SatelliteContractVersion )]

internal static class AssemblyInfo
{
	public const string Major = "2";
	public const string Minor = "2";
	public const string Revision = "1";
	public const string BuildNumber = "0";
	public const string Suffix = "";

	public const string Version = Major + "." + Minor + "." + Revision + ".0";
	public const string FileVersion = Major + "." + Minor + "." + Revision + "." + BuildNumber + Suffix;
	public const string ProductVersion = Major + "." + Minor + "." + Revision;
	public const string SatelliteContractVersion = Major + "." + Minor + ".0.0";
}