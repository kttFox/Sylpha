using System;
using System.Windows;
using Sylpha;
using JetBrains.Annotations;
using Sylpha.Messaging;

namespace ViewLayerSupport.ViewModels {
	public class MessagingWindowViewModel : ViewModel {
		private string _outputMessage;

		public string OutputMessage {
			get { return _outputMessage; }
			set { RaisePropertyChangedIfSet( ref _outputMessage, value ); }
		}

		public async void ConfirmFromViewModel() {
			var message = new ConfirmationMessage( "これはテスト用メッセージです。", "テスト", "MessageKey_Confirm" ) {
				Button = MessageBoxButton.OKCancel
			};
			await Messenger.RaiseAsync( message );
			OutputMessage = $"{DateTime.Now}: ConfirmFromViewModel: {message.Response ?? false}";
		}

		public void ConfirmFromView( [NotNull] ConfirmationMessage message ) {
			if( message == null ) throw new ArgumentNullException( nameof( message ) );

			OutputMessage = $"{DateTime.Now}: ConfirmFromView: {message.Response ?? false}";
		}

		public void FileSelected( [NotNull] OpeningFileSelectionMessage message ) {
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

		public void Initialize() { }
	}
}