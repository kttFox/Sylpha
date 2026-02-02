using System;
using System.Windows;
using JetBrains.Annotations;
using Sylpha;
using Sylpha.Commands;
using Sylpha.Messaging;
using Sylpha.Messaging.Behaviors;

namespace ViewLayerSupport.ViewModels {
	public class MessagingWindowViewModel : ViewModel {
		public void Initialize() { }


		public string OutputMessage {
			get;
			set => RaisePropertyChangedIfSet( ref field, value );
		}

		public async void MessageBoxFromViewModel() {
			var message = new MessageBoxMessage( "これはテスト用メッセージです。", "テスト" ) {
				Button = MessageBoxButton.OKCancel,
				MessageKey = "MessageKey_MessageBox",
			};
			await Messenger.RaiseAsync( message );
			OutputMessage = $"[{DateTime.Now}]: MessageBoxFromViewModel: {message.Response}";
		}

		public void MessageBoxFromView( MessageBoxMessage messageBoxMessage ) {
			if( messageBoxMessage == null ) throw new ArgumentNullException( nameof( messageBoxMessage ) );

			OutputMessage = $"[{DateTime.Now}]: MessageBoxFromView: {messageBoxMessage.Response}";
		}

		public void FileSelected( OpenFileDialogMessage message ) {
			if( message == null ) throw new ArgumentNullException( nameof( message ) );

			var selectedPaths = ( message.Response == null ) ? "未選択" : string.Join( ";", message.Response );
			OutputMessage = $"[{DateTime.Now}][File]: {selectedPaths}";
		}

		public void SaveFileSelected( [NotNull] SaveFileDialogMessage message ) {
			if( message == null ) throw new ArgumentNullException( nameof( message ) );

			var selectedPaths = ( message.Response == null ) ? "未選択" : string.Join( ";", message.Response );
			OutputMessage = $"[{DateTime.Now}][SaveFile]: {selectedPaths}";
		}

		#region OpenFolderDialogCommand
		public ViewModelCommand OpenFolderDialogCommand => field ??= new ViewModelCommand( DoOpenFolderDialogCommand, CanOpenFolderDialogCommand );

		private bool CanOpenFolderDialogCommand() {
#if NET8_0_OR_GREATER
			return true;
#else
			return false;
#endif
		}

		private void DoOpenFolderDialogCommand() {
#if NET8_0_OR_GREATER
			var message = this.Messenger.GetResponse( new OpenFolderDialogMessage() {
				Title = "フォルダを選択してください",
			} );

			var selectedPaths = message.Response == null ? "未選択" : string.Join( ";", message.Response );
			OutputMessage = $"[{DateTime.Now}][Folder]: {selectedPaths}";
#endif
		}
		#endregion

		public void FolderSelected( [NotNull] CommonOpenFileDialogMessage message ) {
			if( message == null ) throw new ArgumentNullException( nameof( message ) );

			var selectedPaths = message.Response == null ? "未選択" : string.Join( ";", message.Response );
			OutputMessage = $"[{DateTime.Now}][Folder]: {selectedPaths}";
		}

	}
}