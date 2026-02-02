using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace Sylpha {
	/// <summary>
	/// UIDispatcherへのアクセスを簡易化します。
	/// </summary>
	public static class DispatcherHelper {
		/// <summary>
		/// UIDispatcherを指定、または取得します。通常このプロパティはApp_StartUpで指定されます。
		/// </summary>
		public static Dispatcher? UIDispatcher {
			get {
				var metadata = DesignerProperties.IsInDesignModeProperty.GetMetadata( typeof(DependencyObject) );
				if( (bool)( metadata?.DefaultValue ?? false ) ) {
					field = Dispatcher.CurrentDispatcher;
				}

				return field;
			}
			set;
		}
	}
}