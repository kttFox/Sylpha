#if NET8_0_OR_GREATER

using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// 「ファイルを開く」ダイアログを表示するアクションです。
	/// </summary>
	public class OpenFolderDialogMessageAction : MessageAction<DependencyObject, OpenFolderDialogMessage> {
		static readonly Dictionary<string, string?> RestoreDirectoryGroupList = [];

		protected override void InvokeAction( OpenFolderDialogMessage message ) {
			Action( AssociatedObject, message );
		}

		public static void Action( DependencyObject element, OpenFolderDialogMessage message ) {
			var window = Window.GetWindow( element );
			var group = message.RestoreDirectoryGroup;

			var initialDirectory = message.InitialDirectory;
			if( !string.IsNullOrEmpty( group ) && RestoreDirectoryGroupList.TryGetValue( group, out var value ) ) {
				initialDirectory = value;
			}

			var dialog = new OpenFolderDialog {
				Title = message.Title,
				InitialDirectory = !string.IsNullOrEmpty( initialDirectory ) ? Path.GetFullPath( initialDirectory ) : initialDirectory,
				Multiselect = message.MultiSelect,
			};

			if( dialog.ShowDialog( window ) == true ) {
				message.Response = dialog.FolderNames;

				if( !string.IsNullOrEmpty( group ) ) {
					RestoreDirectoryGroupList[group] = Path.GetDirectoryName( dialog.FolderName );
				}
			} else {
				message.Response = null;
			}
		}
	}
}
#endif