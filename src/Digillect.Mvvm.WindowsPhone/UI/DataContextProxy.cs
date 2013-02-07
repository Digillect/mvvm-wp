using System;
using System.Windows;
using System.Windows.Data;

namespace Digillect.Mvvm.UI
{
	/// <summary>
	/// Class that serves as a resources-located proxy for DataContext.
	/// </summary>
	public class DataContextProxy : FrameworkElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataContextProxy" /> class.
		/// </summary>
		public DataContextProxy()
		{
			Loaded += delegate( object sender, RoutedEventArgs e )
					{
						var binding = new Binding();

						if( !String.IsNullOrEmpty( BindingPropertyName ) )
						{
							binding.Path = new PropertyPath( BindingPropertyName );
						}

						binding.Source = DataContext;
						binding.Mode = BindingMode;

						SetBinding( DataContextProxy.DataSourceProperty, binding );
					};
		}

		/// <summary>
		/// Gets or sets the data source.
		/// </summary>
		/// <value>
		/// The data source.
		/// </value>
		public object DataSource
		{
			get { return (object) GetValue( DataSourceProperty ); }
			set { SetValue( DataSourceProperty, value ); }
		}

		/// <summary>
		/// The data source property.
		/// </summary>
		public static readonly DependencyProperty DataSourceProperty =
			DependencyProperty.Register( "DataSource", typeof( object ), typeof( DataContextProxy ), null );


		/// <summary>
		/// Gets or sets the name of the binding property.
		/// </summary>
		/// <value>
		/// The name of the binding property.
		/// </value>
		public string BindingPropertyName { get; set; }
		/// <summary>
		/// Gets or sets the binding mode.
		/// </summary>
		/// <value>
		/// The binding mode.
		/// </value>
		public BindingMode BindingMode { get; set; }
	}
}