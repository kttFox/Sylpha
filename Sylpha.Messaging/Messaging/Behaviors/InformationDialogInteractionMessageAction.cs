using System.Windows;
using JetBrains.Annotations;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// 情報ダイアログを表示するアクションです。InformationMessageに対応します。
	/// </summary>
	[PublicAPI]
	public class InformationDialogInteractionMessageAction : InteractionMessageAction<FrameworkElement> {
		protected override void InvokeAction( InteractionMessage message ) {
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