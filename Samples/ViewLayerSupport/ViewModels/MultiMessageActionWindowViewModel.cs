using System.Windows;
using Sylpha;
using Sylpha.Commands;
using Sylpha.Messaging;
using ViewLayerSupport.Views;

namespace ViewLayerSupport.ViewModels {
	public class MultiMessageActionWindowViewModel : ViewModel {

		#region MessageBoxCommand
		public DelegateCommand MessageBoxCommand => field ??= new DelegateCommand( DoMessageBox );

		private void DoMessageBox() {
			this.Messenger.Raise( new MessageBoxMessage( "Information", "info", MessageBoxImage.Information, MessageBoxButton.OK ) );
		}
		#endregion

		#region CallMethodCommand
		public DelegateCommand CallMethodCommand => field ??= new DelegateCommand( DoCallMethodCommand );

		private void DoCallMethodCommand() {
			var r = this.Messenger.Raise( new CallFuncMessage<string>( "GetText" ) );
			_ = r.Result;
		}
		#endregion

		#region ShowWindowCommand
		public DelegateCommand ShowWindowCommand => field ??= new DelegateCommand( DoShowWindowCommand );

		private void DoShowWindowCommand() {
			var r = this.Messenger.Raise( new ShowWindowMessage<MultiMessageActionWindow>() { 
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
		public DelegateCommand OpenFileDialogCommand => field ??= new DelegateCommand( DoOpenFileDialogCommand );

		private void DoOpenFileDialogCommand() {
			var r = this.Messenger.Raise( new OpenFileDialogMessage() );
			_ = r.Response;
		}
		#endregion

		#region SaveFileDialogCommand
		public DelegateCommand SaveFileDialogCommand => field ??= new DelegateCommand( DoSaveFileDialogCommand );

		private void DoSaveFileDialogCommand() {
			var r = this.Messenger.Raise( new SaveFileDialogMessage() );
			_ = r.Response;
		}
		#endregion

		#region OpenFolderDialogCommand
		public DelegateCommand OpenFolderDialogCommand => field ??= new DelegateCommand( DoOpenFolderDialogCommand, CanOpenFolderDialogCommand );

		private bool CanOpenFolderDialogCommand() {
#if NET8_0_OR_GREATER
			return true;
#else
			return false;
#endif
		}

		private void DoOpenFolderDialogCommand() {
#if NET8_0_OR_GREATER
			var r = this.Messenger.Raise( new OpenFolderDialogMessage() );
			_ = r.Response;
#endif
		}
		#endregion

		#region CommonOpenFileDialogCommand
		public DelegateCommand CommonOpenFileDialogCommand => field ??= new DelegateCommand( DoCommonOpenFileDialogCommand );

		private void DoCommonOpenFileDialogCommand() {
			var r = this.Messenger.Raise( new CommonOpenFileDialogMessage() );
			_ = r.Response;
		}
		#endregion

		#region WindowActionCommand
		public DelegateCommand WindowActionCommand => field ??= new DelegateCommand( DoWindowActionCommand );

		private void DoWindowActionCommand() {
			this.Messenger.Raise( new WindowActionMessage( WindowActionMode.Maximize ) );
		}
		#endregion
	}
}