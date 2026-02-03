using System.Globalization;
using System.Windows;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// メッセージボックスを表示するアクション
	/// </summary>
	public class MessageBoxMessageAction : MessageAction<FrameworkElement, MessageBoxMessage> {
		/// <inheritdoc />
		protected override void InvokeAction( MessageBoxMessage message ) {
			Action( AssociatedObject, message );
		}

		/// <summary>
		/// メッセージボックスを表示するアクション
		/// </summary>
		/// <param name="element">対象の<see cref="DependencyObject"/></param>
		/// <param name="message">ダイアログの設定と結果を格納するメッセージ</param>
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