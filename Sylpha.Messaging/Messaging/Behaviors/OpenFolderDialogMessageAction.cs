#if NET8_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// 「ファイルを開く」ダイアログを表示するアクションです。
	/// </summary>
	public class OpenFolderDialogMessageAction : MessageAction<DependencyObject, OpenFolderDialogMessage> {
		protected override void InvokeAction( OpenFolderDialogMessage message ) {
			Action( AssociatedObject, message );
		}

		public static void Action( DependencyObject element, OpenFolderDialogMessage message ) {
			var window = Window.GetWindow( element );

			var dialog = new OpenFolderDialog {
				Title = message.Title,
				InitialDirectory = message.InitialDirectory,
				Multiselect = message.MultiSelect,
			};

			if( dialog.ShowDialog( window ) == true ) {
				message.Response = dialog.FolderNames;
			} else {
				message.Response = null;
			}
		}
	}
}
#endif