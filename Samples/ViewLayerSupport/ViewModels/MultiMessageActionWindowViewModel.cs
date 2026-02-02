using System.Windows;
using Sylpha;
using Sylpha.Commands;
using Sylpha.Messaging;
using ViewLayerSupport.Views;

namespace ViewLayerSupport.ViewModels {
	public class MultiMessageActionWindowViewModel : ViewModel {
		public void Initialize() { }

		#region MessageBoxCommand
		public ViewModelCommand MessageBoxCommand => field ??= new ViewModelCommand( DoMessageBox );

		private void DoMessageBox() {
			this.Messenger.Raise( new MessageBoxMessage( "Information", "info", MessageBoxImage.Information, MessageBoxButton.OK ) );
		}
		#endregion

		#region CallMethodCommand
		public ViewModelCommand CallMethodCommand => field ??= new ViewModelCommand( DoCallMethodCommand );

		private void DoCallMethodCommand() {
			var r = this.Messenger.GetResponse( new CallFuncMessage<string>( "GetText" ) );
			_ = r.Result;
		}
		#endregion

		#region ShowWindowCommand
		public ViewModelCommand ShowWindowCommand => field ??= new ViewModelCommand( DoShowWindowCommand );

		private void DoShowWindowCommand() {
			var r = this.Messenger.GetResponse( new ShowWindowMessage( typeof( MultiMessageActionWindow ) ) { 
				WindowSettingAction = w => { 
					w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
					w.Title = "Shown by ShowWindowMessage";
					w.IsEnabled = false;
				},
				InitializeAction = w => { 
					w.Background = System.Windows.Media.Brushes.Red;
				},
			} );
			_ = r.Response;
		}
		#endregion

		#region OpenFileDialogCommand
		public ViewModelCommand OpenFileDialogCommand => field ??= new ViewModelCommand( DoOpenFileDialogCommand );

		private void DoOpenFileDialogCommand() {
			var r = this.Messenger.GetResponse( new OpenFileDialogMessage() );
			_ = r.Response;
		}
		#endregion

		#region SaveFileDialogCommand
		public ViewModelCommand SaveFileDialogCommand => field ??= new ViewModelCommand( DoSaveFileDialogCommand );

		private void DoSaveFileDialogCommand() {
			var r = this.Messenger.GetResponse( new SaveFileDialogMessage() );
			_ = r.Response;
		}
		#endregion

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
			var r = this.Messenger.GetResponse( new OpenFolderDialogMessage() );
			_ = r.Response;
#endif
		}
		#endregion

		#region CommonOpenFileDialogCommand
		public ViewModelCommand CommonOpenFileDialogCommand => field ??= new ViewModelCommand( DoCommonOpenFileDialogCommand );

		private void DoCommonOpenFileDialogCommand() {
			var r = this.Messenger.GetResponse( new CommonOpenFileDialogMessage() );
			_ = r.Response;
		}
		#endregion

		#region WindowActionCommand
		public ViewModelCommand WindowActionCommand => field ??= new ViewModelCommand( DoWindowActionCommand );

		private void DoWindowActionCommand() {
			this.Messenger.Raise( new WindowActionMessage( WindowActionMode.Maximize ) );
		}
		#endregion
	}
}