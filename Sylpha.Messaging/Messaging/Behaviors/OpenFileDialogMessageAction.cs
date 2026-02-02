using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// 「ファイルを開く」ダイアログを表示するアクションです。
	/// </summary>
	public class OpenFileDialogMessageAction : MessageAction<DependencyObject, OpenFileDialogMessage> {
		static readonly Dictionary<string, string?> RestoreDirectoryGroupList = [];

		protected override void InvokeAction( OpenFileDialogMessage message ) {
			Action( AssociatedObject, message );
		}

		public static void Action( DependencyObject element, OpenFileDialogMessage message ) {
			var window = Window.GetWindow( element );
			var group = message.RestoreDirectoryGroup;

			var initialDirectory = message.InitialDirectory;
			if( !string.IsNullOrEmpty( group ) && RestoreDirectoryGroupList.TryGetValue( group, out var value ) ) {
				initialDirectory = value;
			}

			var dialog = new OpenFileDialog() {
				Title = message.Title,
				InitialDirectory = !string.IsNullOrEmpty( initialDirectory ) ? Path.GetFullPath( initialDirectory ) : initialDirectory,
				FilterIndex = message.FilterIndex,
				Filter = message.Filter,
				
				FileName = message.FileName,
				DefaultExt = message.DefaultExt,
				CheckPathExists = message.CheckPathExists,
				CheckFileExists = message.CheckFileExists,
				AddExtension = message.AddExtension,
				Multiselect = message.MultiSelect,
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