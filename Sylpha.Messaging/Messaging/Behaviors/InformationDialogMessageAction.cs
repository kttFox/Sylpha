using System.Windows;
using JetBrains.Annotations;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// 情報ダイアログを表示するアクションです。InformationMessageに対応します。
	/// </summary>
	[PublicAPI]
	public class InformationDialogMessageAction : MessageAction<FrameworkElement> {
		protected override void InvokeAction( Message message ) {
			if( message is InformationMessage informationMessage ) {
				MessageBox.Show(
					informationMessage.Text,
					informationMessage.Caption,
					MessageBoxButton.OK,
					informationMessage.Image
				);
			}
		}
	}
}