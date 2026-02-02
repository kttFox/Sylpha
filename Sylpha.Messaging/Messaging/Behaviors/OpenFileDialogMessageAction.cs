using System.Windows;
using Microsoft.Win32;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// 「ファイルを開く」ダイアログを表示するアクションです。<see cref="OpeningFileSelectionMessage" />に対応します。
	/// </summary>
	public class OpenFileDialogMessageAction : MessageAction<DependencyObject> {
		protected override void InvokeAction( Message message ) {
			// ReSharper disable once InvertIf
			if( message is OpeningFileSelectionMessage openFileMessage ) {
				var dialog = new OpenFileDialog {
					FileName = openFileMessage.FileName,
					InitialDirectory = openFileMessage.InitialDirectory,
					AddExtension = openFileMessage.AddExtension,
					Filter = openFileMessage.Filter,
					Title = openFileMessage.Title,
					Multiselect = openFileMessage.MultiSelect
				};

				var showDialog = dialog.ShowDialog() ?? false;
				openFileMessage.Response = showDialog ? dialog.FileNames : null;
			}
		}
	}
}