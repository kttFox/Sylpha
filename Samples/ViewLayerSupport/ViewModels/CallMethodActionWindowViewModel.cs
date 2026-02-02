using System;
using Sylpha;
using Sylpha.Commands;
using Sylpha.Messaging;

namespace ViewLayerSupport.ViewModels {
	public class CallMethodActionWindowViewModel : ViewModel {
		public void Initialize() { }

		public string Text {
			get;
			set => RaisePropertyChangedIfSet( ref field, value );
		}

		void ButtonClick() {
			this.Text = $"[{DateTime.Now}] ViewModel - Button Clicked!";
		}

		void ButtonClick( string? param ) {
			this.Text = $"[{DateTime.Now}] ViewModel - Button Clicked with param: {param ?? "null"}";
		}


		#region ButtonClickCommand
		public ViewModelCommand ButtonClickCommand => field ??= new ViewModelCommand( DoButtonClickCommand );

		private void DoButtonClickCommand() {
			this.Messenger.Raise( new CallActionMessage( "ButtonClick" ) );
		}
		#endregion

		#region ButtonClickCommand2
		public ListenerCommand<string> ButtonClickCommand2 => field ??= new ListenerCommand<string>( DoButtonClickCommand2 );

		private void DoButtonClickCommand2( string? param ) {
			this.Messenger.Raise( new CallActionMessage<string?>( "ButtonClick", param ) );
		}
		#endregion

		#region ButtonClickCommand3
		public ViewModelCommand ButtonClickCommand3 => field ??= new ViewModelCommand( DoButtonClickCommand3 );

		private void DoButtonClickCommand3() {
			var r = this.Messenger.GetResponse( new CallFuncMessage<string>( "GetText" ) );
			this.Text = $"[{DateTime.Now}] {r.Result}";
		}
		#endregion

		#region ButtonClickCommand3
		public ListenerCommand<string> ButtonClickCommand4 => field ??= new ListenerCommand<string>( DoButtonClickCommand4 );

		private void DoButtonClickCommand4( string? param ) {
			var r = this.Messenger.GetResponse( new CallFuncMessage<string, string>( "GetText", param ?? "null" ) );
			this.Text = $"[{DateTime.Now}] {r.Result}";
		}
		#endregion
	}
}