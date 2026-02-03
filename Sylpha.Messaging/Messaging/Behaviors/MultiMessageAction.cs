using System.Windows;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// 複数のメッセージアクションをまとめて扱うアクション
	/// </summary>
	public class MultiMessageAction : MessageAction<FrameworkElement, Message> {
		/// <inheritdoc />
		protected override void InvokeAction( Message message ) {
			switch( message ) {
				case MessageBoxMessage msg: {
					MessageBoxMessageAction.Action( this.AssociatedObject, msg );
					return;
				}

				case ShowWindowMessage msg: {
					ShowWindowMessageAction.Action( this.AssociatedObject, msg );
					return;
				}

				case OpenFileDialogMessage msg: {
					OpenFileDialogMessageAction.Action( this.AssociatedObject, msg );
					return;
				}

				case SaveFileDialogMessage msg: {
					SaveFileDialogMessageAction.Action( this.AssociatedObject, msg );
					return;
				}
#if NET8_0_OR_GREATER
				case OpenFolderDialogMessage msg: {
					OpenFolderDialogMessageAction.Action( this.AssociatedObject, msg );
					return;
				}
#endif
				case WindowActionMessage msg: {
					WindowActionMessageAction.Action( this.AssociatedObject, msg );
					return;
				}

				case CallMethodMessage msg: {
					CallMethodAction.Action( this.AssociatedObject, null, null, null, msg );
					return;
				}
			}
		}
	}
}
