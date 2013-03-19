using System;
using System.Globalization;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	/// Helper to pass parameters in URI.
	/// </summary>
	public static class ParametersSerializer
	{
		const string DateTimeFormatString = "yyyy-MM-ddThh:mm:sszzz";

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

			Type valueType = value.GetType();
			string formattedValue;

			if( valueType == typeof( DateTime ) )
			{
				var dtValue = (DateTime) value;

				formattedValue = dtValue.ToString( DateTimeFormatString, CultureInfo.InvariantCulture );
			}
			else
			{
				if( valueType == typeof( DateTimeOffset ) )
				{
					var dtValue = (DateTimeOffset) value;

					formattedValue = dtValue.ToString( DateTimeFormatString, CultureInfo.InvariantCulture );
				}
				else if( valueType == typeof( XKey ) )
				{
					formattedValue = XKeySerializer.Serialize( (XKey) value );
				}
				else
				{
					formattedValue = (string) Convert.ChangeType( value, typeof( string ), CultureInfo.InvariantCulture );
				}
			}

			return Uri.EscapeDataString( formattedValue );
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

			try
			{
				if( targetType == typeof( DateTime ) )
				{
					return DateTime.ParseExact( stringValue, DateTimeFormatString, CultureInfo.InvariantCulture );
				}

				if( targetType == typeof( DateTimeOffset ) )
				{
					return DateTimeOffset.ParseExact( stringValue, DateTimeFormatString, CultureInfo.InvariantCulture );
				}

				if( targetType == typeof( XKey ) )
				{
					return XKeySerializer.Deserialize( stringValue );
				}

				return Convert.ChangeType( stringValue, targetType, CultureInfo.InvariantCulture );
			}
			catch( Exception )
			{
				return null;
			}
		}
	}
}