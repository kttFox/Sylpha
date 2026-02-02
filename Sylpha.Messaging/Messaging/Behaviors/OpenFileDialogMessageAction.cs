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
	public class OpenFileDialogMessageAction : MessageAction<DependencyObject, OpenFileDialogMessage> {
		protected override void InvokeAction( OpenFileDialogMessage message ) {
			Action( AssociatedObject, message );
		}

		public static void Action( DependencyObject element, OpenFileDialogMessage message ) {
			var window = Window.GetWindow( element );

			var dialog = new OpenFileDialog() {
				Title = message.Title,
				InitialDirectory = message.InitialDirectory,
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
			} else {
				message.Response = null;
			}
		}
	}
}