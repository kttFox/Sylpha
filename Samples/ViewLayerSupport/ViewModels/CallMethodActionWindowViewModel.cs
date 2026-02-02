using System;
using Sylpha;
using Sylpha.Commands;
using Sylpha.Messaging;

namespace ViewLayerSupport.ViewModels {
	public class CallMethodActionWindowViewModel : ViewModel {
		public void Initialize() { }

		public string Text { get; set => SetProperty( ref field, value ); }

		void ButtonClick() {
			this.Text = $"[{DateTime.Now}] ViewModel - Button Clicked!";
		}

		void ButtonClick( string? param ) {
			this.Text = $"[{DateTime.Now}] ViewModel - Button Clicked with param: {param ?? "null"}";
		}


		#region ButtonClickCommand
		public DelegateCommand ButtonClickCommand => field ??= new DelegateCommand( DoButtonClickCommand );

		private void DoButtonClickCommand() {
			this.Messenger.Raise( new CallActionMessage( "ButtonClick" ) );
		}
		#endregion

		#region ButtonClickCommand2
		public DelegateCommand<string> ButtonClickCommand2 => field ??= new DelegateCommand<string>( DoButtonClickCommand2 );

		private void DoButtonClickCommand2( string? param ) {
			this.Messenger.Raise( new CallActionMessage<string?>( "ButtonClick", param ) );
		}
		#endregion

		#region ButtonClickCommand3
		public DelegateCommand ButtonClickCommand3 => field ??= new DelegateCommand( DoButtonClickCommand3 );

		private void DoButtonClickCommand3() {
			var r = this.Messenger.Raise( new CallFuncMessage<string>( "GetText" ) );
			this.Text = $"[{DateTime.Now}] {r.Result}";
		}
		#endregion

		#region ButtonClickCommand3
		public DelegateCommand<string> ButtonClickCommand4 => field ??= new DelegateCommand<string>( DoButtonClickCommand4 );

		private void DoButtonClickCommand4( string? param ) {
			var r = this.Messenger.Raise( new CallFuncMessage<string, string>( "GetText", param ?? "null" ) );
			this.Text = $"[{DateTime.Now}] {r.Result}";
		}
		#endregion
	}
}