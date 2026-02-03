using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// ウィンドウの表示を行うアクション
	/// </summary>
	public class ShowWindowMessageAction : MessageAction<FrameworkElement, ShowWindowMessage> {
		/// <inheritdoc />
		protected override void InvokeAction( ShowWindowMessage message ) {
			Action( AssociatedObject, message );
		}

		/// <summary>
		/// ウィンドウの表示を行うアクション
		/// </summary>
		/// <param name="element">対象の<see cref="DependencyObject"/></param>
		/// <param name="message">ダイアログの設定と結果を格納するメッセージ</param>
		public static void Action( FrameworkElement element, ShowWindowMessage message ) {
			var targetType = message.WindowType;
			var defaultConstructor = GetConstructor( targetType ) ?? throw new Exception();

			switch( message.Mode ) {
				case ShowWindowMode.Modeless:
				case ShowWindowMode.Modal: {
					ShowWindow( message.Mode );
					break;
				}
				case ShowWindowMode.NewOrActive: {
					var window = Application.Current.Windows
									.OfType<Window>()
									.FirstOrDefault( w => w.GetType() == targetType );

					if( window == null ) {
						// 既存ウィンドウが無ければ新規表示
						ShowWindow( ShowWindowMode.Modeless );

					} else {
						if( message.ViewModel != null ) {
							window.DataContext = message.ViewModel;
						}
						if( message.IsOwned == true ) {
							window.Owner = Window.GetWindow( element );
						}
						message.WindowSettingAction?.Invoke( window );
						window.Activate();

						message.Response = null;
						message.WindowViewModel = window.DataContext as INotifyPropertyChanged;
					}

					break;
				}

				default: {
					break;
				}
			}

			void ShowWindow( ShowWindowMode mode ) {
				var window = (Window)defaultConstructor.Invoke( null );
				if( message.ViewModel != null ) {
					window.DataContext = message.ViewModel;
				}

				if( message.IsOwned == true ) {
					window.Owner = Window.GetWindow( element );
				}

				message.WindowSettingAction?.Invoke( window );

				window.ContentRendered += ( x, e ) => {
					message.InitializeAction?.Invoke( window );
				};

				if( mode == ShowWindowMode.Modeless ) {
					window.Show();
					message.Response = null;

				} else {
					window.StateChanged += ( s, e ) => {
						var target = (Window)s!;
						if( target.WindowState == WindowState.Minimized ) {
							target.Owner?.WindowState = WindowState.Minimized;
						}
					};

					message.Response = window.ShowDialog();
				}

				message.WindowViewModel = window.DataContext as INotifyPropertyChanged;
			}
		}

		private static ConstructorInfo? GetConstructor( Type? value ) {
			if( value?.IsSubclassOf( typeof( Window ) ) == true ) {
				return value.GetConstructor( Type.EmptyTypes );
			}
			return null;
		}
	}
}