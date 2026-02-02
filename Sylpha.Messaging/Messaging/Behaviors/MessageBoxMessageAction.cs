using System.Globalization;
using System.Windows;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// 確認ダイアログを表示するアクションです。
	/// </summary>
	public class MessageBoxMessageAction : MessageAction<FrameworkElement, MessageBoxMessage> {
		protected override void InvokeAction( MessageBoxMessage message ) {
			Action( AssociatedObject, message );
		}

		public static void Action( FrameworkElement element, MessageBoxMessage message ) {
			if( message.IsOwned ) {
				message.Response = MessageBox.Show(
					Window.GetWindow( element ),
					message.Text,
					message.Caption,
					message.Button,
					message.Image,
					message.DefaultResult
				);
			} else {
				message.Response = MessageBox.Show(
					message.Text,
					message.Caption,
					message.Button,
					message.Image,
					message.DefaultResult
				);
			}
		}
	}
}