using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// フォルダ選択ダイアログを表示するアクション
	/// </summary>
	public class CommonOpenFileDialogMessageAction : MessageAction<FrameworkElement, CommonOpenFileDialogMessage> {
		static readonly Dictionary<string, string?> RestoreDirectoryGroupList = [];

		/// <inheritdoc />
		protected override void InvokeAction( CommonOpenFileDialogMessage message ) {
			Action( AssociatedObject, message );
		}

		/// <summary>
		/// フォルダ選択ダイアログを表示するアクション
		/// </summary>
		/// <param name="element">対象の<see cref="DependencyObject"/></param>
		/// <param name="message">ダイアログの設定と結果を格納するメッセージ</param>
		public static void Action( FrameworkElement element, CommonOpenFileDialogMessage message ) {
			if( !CommonFileDialog.IsPlatformSupported ) return;

			var window = Window.GetWindow( element );
			var group = message.RestoreDirectoryGroup;

			var initialDirectory = message.InitialDirectory;
			if( !string.IsNullOrEmpty( group ) && RestoreDirectoryGroupList.TryGetValue( group, out var value ) ) {
				initialDirectory = value;
			}

			using var dialog = new CommonOpenFileDialog();
			dialog.IsFolderPicker = true;
			dialog.Title = message.Title;
			dialog.InitialDirectory = !string.IsNullOrEmpty( initialDirectory ) ? Path.GetFullPath( initialDirectory ) : initialDirectory;
			dialog.Multiselect = message.Multiselect;

			if( dialog.ShowDialog( window ) == CommonFileDialogResult.Ok ) {
				message.Response = dialog.FileNames.ToArray();

				if( !string.IsNullOrEmpty( group ) ) {
					RestoreDirectoryGroupList[group] = Path.GetDirectoryName( dialog.FileName );
				}
			} else {
				message.Response = null;
			}
		}
	}
}