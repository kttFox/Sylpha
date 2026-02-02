using System;
using System.Linq;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// フォルダ選択ダイアログを表示するアクションです。
	/// </summary>
	public class CommonOpenFileDialogMessageAction : MessageAction<FrameworkElement, CommonOpenFileDialogMessage> {
		protected override void InvokeAction( CommonOpenFileDialogMessage message ) {
			Action( AssociatedObject, message );
		}

		public static void Action( FrameworkElement element, CommonOpenFileDialogMessage message ) {
			if( !CommonFileDialog.IsPlatformSupported ) return;

			var hostWindow = Window.GetWindow( element ?? throw new InvalidOperationException() );
			if( hostWindow == null ) return;

			using var dialog = new CommonOpenFileDialog();
			dialog.IsFolderPicker = true;
			dialog.Title = message.Title;
			dialog.InitialDirectory = message.InitialDirectory;
			dialog.Multiselect = message.Multiselect;

			if( dialog.ShowDialog( hostWindow ) == CommonFileDialogResult.Ok ) {
				message.Response = dialog.FileNames.ToArray();
			} else {
				message.Response = null;
			}
		}
	}
}