using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// 「ファイルを保存する」ダイアログを表示するアクションです。
	/// </summary>
	public class SaveFileDialogMessageAction : MessageAction<DependencyObject, SaveFileDialogMessage> {
		static readonly Dictionary<string, string?> RestoreDirectoryGroupList = [];

		protected override void InvokeAction( SaveFileDialogMessage message ) {
			Action( this.AssociatedObject, message );
		}

		public static void Action( DependencyObject element, SaveFileDialogMessage message ) {
			var window = Window.GetWindow( element );
			var group = message.RestoreDirectoryGroup;

			var initialDirectory = message.InitialDirectory;
			if( !string.IsNullOrEmpty( group ) && RestoreDirectoryGroupList.TryGetValue( group, out var value ) ) {
				initialDirectory = value;
			}

			var dialog = new SaveFileDialog {
				Title = message.Title,
				InitialDirectory = !string.IsNullOrEmpty( initialDirectory ) ? Path.GetFullPath( initialDirectory ) : initialDirectory,
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

				if( !string.IsNullOrEmpty( group ) ) {
					RestoreDirectoryGroupList[group] = Path.GetDirectoryName( dialog.FileName );
				}
			} else {
				message.Response = null;
			}
		}
	}
}