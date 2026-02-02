using System;
using System.Windows;
using Sylpha.Dialogs;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// Show folder browser dialog.
	/// <see cref="MessageAction{T}" /> for <see cref="FolderSelectionMessage" />.
	/// This object must be hosted by <see cref="FrameworkElement" />.
	/// </summary>
	public class FolderBrowserDialogMessageAction : MessageAction<FrameworkElement> {
		/// <summary>
		/// Invokes the action related to this class.
		/// </summary>
		/// <param name="m"><see cref="FolderSelectionMessage" /> specified to <see cref="Messenger" /> in the client.</param>
		protected override void InvokeAction( Message m ) {
			// ReSharper disable once InvertIf
			if( m is FolderSelectionMessage folderSelectionMessage ) {
				var hostWindow = Window.GetWindow( AssociatedObject ?? throw new InvalidOperationException() );
				if( hostWindow == null ) return;

				using( var dialog = FolderSelectionDialogFactory.CreateDialog( folderSelectionMessage.DialogPreference ) ) {
					if( dialog == null ) throw new InvalidOperationException();

					dialog.Title = folderSelectionMessage.Title;
					dialog.Description = folderSelectionMessage.Description;
					dialog.SelectedPath = folderSelectionMessage.SelectedPath;
					dialog.Multiselect = folderSelectionMessage.Multiselect;

					if( dialog.ShowDialog( hostWindow ).GetValueOrDefault() ) {
						folderSelectionMessage.Response = dialog.SelectedPaths;
					} else {
						folderSelectionMessage.Response = null;
					}
				}
			}
		}
	}
}