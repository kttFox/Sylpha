using Sylpha;

namespace ViewLayerSupport.ViewModels {
	public class CallMethodActionWindowViewModel : ViewModel {
		private string _message;

		public string Message {
			get { return _message; }
			private set { RaisePropertyChangedIfSet( ref _message, value ); }
		}

		public void TextChanged( string text ) {
			Message = $"TextChanged: {text}";
		}

		public void Initialize() { }
	}
}