using System;
using System.Windows;
using Sylpha.Dialogs;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// Show folder browser dialog.
	/// </summary>
	public class FolderBrowserDialogMessageAction : MessageAction<FrameworkElement, FolderSelectionMessage> {
		protected override void InvokeAction( FolderSelectionMessage message ) {
			var hostWindow = Window.GetWindow( AssociatedObject ?? throw new InvalidOperationException() );
			if( hostWindow == null ) return;

			using( var dialog = FolderSelectionDialogFactory.CreateDialog( message.DialogPreference ) ) {
				if( dialog == null ) throw new InvalidOperationException();

				dialog.Title = message.Title;
				dialog.Description = message.Description;
				dialog.SelectedPath = message.SelectedPath;
				dialog.Multiselect = message.Multiselect;

				if( dialog.ShowDialog( hostWindow ).GetValueOrDefault() ) {
					message.Response = dialog.SelectedPaths;
				} else {
					message.Response = null;
				}
			}
		}
	}
}