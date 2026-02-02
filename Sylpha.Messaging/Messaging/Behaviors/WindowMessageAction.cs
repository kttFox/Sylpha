using System;
using System.Windows;
using JetBrains.Annotations;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// Windowの最小化・最大化・閉じるを行うアクションです。WindowActionMessageに対応します。
	/// </summary>
	[PublicAPI]
	public class WindowInteractionMessageAction : InteractionMessageAction<FrameworkElement> {
		protected override void InvokeAction( InteractionMessage message ) {
			// ReSharper disable once InvertIf
			if( message is WindowActionMessage windowMessage && AssociatedObject != null ) {
				var window = Window.GetWindow( AssociatedObject );
				if( window == null ) return;

				switch( windowMessage.Action ) {
					case WindowAction.Close:
						window.Close();
						break;
					case WindowAction.Maximize:
						window.WindowState = WindowState.Maximized;
						break;
					case WindowAction.Minimize:
						window.WindowState = WindowState.Minimized;
						break;
					case WindowAction.Normal:
						window.WindowState = WindowState.Normal;
						break;
					case WindowAction.Active:
						window.Activate();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}