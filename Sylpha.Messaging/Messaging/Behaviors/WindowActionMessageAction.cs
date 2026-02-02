using System;
using System.Windows;
using JetBrains.Annotations;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// Windowの最小化、最大化、閉じる などを行うアクションです。
	/// </summary>
	[PublicAPI]
	public class WindowActionMessageAction : MessageAction<FrameworkElement, WindowActionMessage> {
		protected override void InvokeAction( WindowActionMessage message ) {
			Action( this.AssociatedObject, message );
		}

		public static void Action( FrameworkElement element, WindowActionMessage message ) {
			var window = Window.GetWindow( element );
			if( window != null ) {
				switch( message.WindowAction ) {
					case WindowActionMode.Close: {
						window.Close();
						break;
					}
					case WindowActionMode.Maximize: {
						window.WindowState = WindowState.Maximized;
						break;
					}
					case WindowActionMode.Minimize: {
						window.WindowState = WindowState.Minimized;
						break;
					}
					case WindowActionMode.Normal: {
						window.WindowState = WindowState.Normal;
						break;
					}
					case WindowActionMode.Active: {
						window.Activate();
						break;
					}
					case WindowActionMode.Hide: {
						window.Hide();
						break;
					}
					case WindowActionMode.ResultOK: {
						window.DialogResult = true;
						break;
					}
					case WindowActionMode.ResultCancel: {
						window.DialogResult = false;
						break;
					}

					default: {
						break;
					}
				}
			}
		}
	}
}