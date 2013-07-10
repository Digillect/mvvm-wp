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
using System.Globalization;
using System.Linq;

using Digillect.Runtime.Serialization;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	///     Helper to pass parameters in URI.
	/// </summary>
	public static class ParametersSerializer
	{
		/// <summary>
		///     Encodes the value to string representation.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>Encoded value.</returns>
		public static string EncodeValue( object value )
		{
			if( value == null )
			{
				return null;
			}

			var valueType = value.GetType();
			string formattedValue;

			if( valueType.GetInterfaces().Contains( typeof( ISerializableParameter ) ) )
			{
				formattedValue = ((ISerializableParameter) value).SerializeToString();
			}
			else
			{
				if( valueType == typeof( DateTime ) )
				{
					formattedValue = ((DateTime) value).ToString( "o", DateTimeFormatInfo.InvariantInfo );
				}
				else if( valueType == typeof( DateTimeOffset ) )
				{
					formattedValue = ((DateTimeOffset) value).ToString( "o", DateTimeFormatInfo.InvariantInfo );
				}
				else if( valueType == typeof( XKey ) )
				{
					formattedValue = XKeySerializer.Serialize( (XKey) value );
				}
				else if( valueType.IsEnum )
				{
					formattedValue = ((int) value).ToString( CultureInfo.InvariantCulture );
				}
				else
				{
					formattedValue = (string) Convert.ChangeType( value, typeof( string ), CultureInfo.InvariantCulture );
				}
			}

			return formattedValue == null ? null : Uri.EscapeDataString( formattedValue );
		}

		/// <summary>
		///     Decodes the value.
		/// </summary>
		/// <param name="stringValue">String representation of the value.</param>
		/// <param name="targetType">Target value type.</param>
		/// <returns>Decoded value.</returns>
		public static object DecodeValue( string stringValue, Type targetType )
		{
			if( string.IsNullOrEmpty( stringValue ) || targetType == typeof( string ) )
			{
				return stringValue;
			}

			if( targetType.GetInterfaces().Contains( typeof( ISerializableParameter ) ) )
			{
				var target = (ISerializableParameter) Activator.CreateInstance( targetType );

				target.DeserializeFromString( stringValue );

				return target;
			}

			if( targetType == typeof( DateTime ) )
			{
				return DateTime.ParseExact( stringValue, "o", DateTimeFormatInfo.InvariantInfo );
			}

			if( targetType == typeof( DateTimeOffset ) )
			{
				return DateTimeOffset.ParseExact( stringValue, "o", DateTimeFormatInfo.InvariantInfo );
			}

			if( targetType == typeof( XKey ) )
			{
				return XKeySerializer.Deserialize( stringValue );
			}

			if( targetType.IsEnum )
			{
				return Enum.ToObject( targetType, int.Parse( stringValue, CultureInfo.InvariantCulture ) );
			}

			return Convert.ChangeType( stringValue, targetType, CultureInfo.InvariantCulture );
		}
	}
}