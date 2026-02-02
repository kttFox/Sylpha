using System;
using System.Windows;
using Sylpha;
using JetBrains.Annotations;
using Sylpha.Messaging;

namespace ViewLayerSupport.ViewModels {
	public class MessagingWindowViewModel : ViewModel {
		public void Initialize() { }


		private string _outputMessage;

		public string OutputMessage {
			get { return _outputMessage; }
			set { RaisePropertyChangedIfSet( ref _outputMessage, value ); }
		}

		public async void MessageBoxFromViewModel() {
			var message = new MessageBoxMessage( "これはテスト用メッセージです。", "テスト" ) {
				Button = MessageBoxButton.OKCancel,
				MessageKey = "MessageKey_MessageBox",
			};
			await Messenger.RaiseAsync( message );
			OutputMessage = $"{DateTime.Now}: MessageBoxFromViewModel: {message.Response}";
		}

		public void MessageBoxFromView( MessageBoxMessage messageBoxMessage ) {
			if( messageBoxMessage == null ) throw new ArgumentNullException( nameof( messageBoxMessage ) );

			OutputMessage = $"{DateTime.Now}: MessageBoxFromView: {messageBoxMessage.Response}";
		}

		public void FileSelected( OpenFileDialogMessage message ) {
			if( message == null ) throw new ArgumentNullException( nameof( message ) );


			string selectedPaths = message.Response == null
				? "未選択"
				: String.Join( ";", message.Response );
			OutputMessage = $"{DateTime.Now}: FileSelected: {selectedPaths}";
		}
		public void FolderSelected( [NotNull] FolderSelectionMessage message ) {
			if( message == null ) throw new ArgumentNullException( nameof( message ) );

			string selectedPaths = message.Response == null
				? "未選択"
				: String.Join( ";", message.Response );

			OutputMessage = $"{DateTime.Now}: FolderSelected: {selectedPaths}";
		}
		
	}
}