using System.Collections.Generic;
using System.IO;
using System.Windows;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// 「ファイルを保存する」ダイアログを表示するアクションです。
	/// </summary>
	[PublicAPI]
	public class SaveFileDialogMessageAction : MessageAction<DependencyObject, SaveFileDialogMessage> {
		protected override void InvokeAction( SaveFileDialogMessage message ) {
			Action( this.AssociatedObject, message );
		}

		public static void Action( DependencyObject element, SaveFileDialogMessage message ) {
			var window = Window.GetWindow( element );
			
			var dialog = new SaveFileDialog {
				Title = message.Title,
				InitialDirectory = message.InitialDirectory,
				FilterIndex = message.FilterIndex,
				Filter = message.Filter,
				FileName = message.FileName,
				DefaultExt = message.DefaultExt,
				CheckPathExists = message.CheckPathExists,
				CheckFileExists = message.CheckFileExists,
				AddExtension = message.AddExtension,
				CreatePrompt = message.CreatePrompt,
#if NET8_0_OR_GREATER
				CreateTestFile = message.CreateTestFile,
#endif
				OverwritePrompt = message.OverwritePrompt,
			};

			if( dialog.ShowDialog( window ) == true ) {
				message.Response = dialog.FileNames;
			} else {
				message.Response = null;
			}
		}
	}
}