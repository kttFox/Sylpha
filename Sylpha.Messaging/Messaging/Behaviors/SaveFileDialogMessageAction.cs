using System.Windows;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// 「ファイルを保存する」ダイアログを表示するアクションです。<see cref="SavingFileSelectionMessage" />に対応します。
	/// </summary>
	[PublicAPI]
	public class SaveFileDialogMessageAction : MessageAction<DependencyObject> {
		protected override void InvokeAction( Message message ) {
			// ReSharper disable once InvertIf
			if( message is SavingFileSelectionMessage saveFileMessage ) {
				var dialog = new SaveFileDialog {
					FileName = saveFileMessage.FileName,
					InitialDirectory = saveFileMessage.InitialDirectory,
					AddExtension = saveFileMessage.AddExtension,
					CreatePrompt = saveFileMessage.CreatePrompt,
					Filter = saveFileMessage.Filter,
					OverwritePrompt = saveFileMessage.OverwritePrompt,
					Title = saveFileMessage.Title
				};

				var showDialog = dialog.ShowDialog() ?? false;
				saveFileMessage.Response = showDialog ? dialog.FileNames : null;
			}
		}
	}
}