using System.Windows;

namespace Sylpha.Messaging {
	/// <summary>
	/// ファイルを開く アクション用のメッセージです。
	/// </summary>
	public class OpenFileDialogMessage : FileDialogMessage {
		#region Register MultiSelect
		/// <summary>
		/// 複数ファイルを選択可能かを取得、または設定します。
		/// </summary>
		public bool MultiSelect {
			get { return (bool)( GetValue( MultiSelectProperty ) ); }
			set { SetValue( MultiSelectProperty, value ); }
		}

		public static readonly DependencyProperty MultiSelectProperty =
			DependencyProperty.Register( nameof( MultiSelect ), typeof( bool ), typeof( OpenFileDialogMessage ), new PropertyMetadata( false ) );
		#endregion


		/// <summary>
		/// 新しいメッセージのインスタンスを生成します。
		/// </summary>
		public OpenFileDialogMessage() { }

		/// <summary>
		/// メッセージキーを指定して、新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public OpenFileDialogMessage( string? messageKey ) : base( messageKey ) { }

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		protected override Freezable CreateInstanceCore() => new OpenFileDialogMessage();

	}

	public static class OpenFileDialogMessageExtensions {
		/// <summary>
		/// ファイルを開くダイアログを表示するメッセージを送信し、結果を取得します。
		/// </summary>
		/// <param name="messenger">送信先</param>
		/// <param name="message">送信するメッセージ</param>
		/// <returns>結果が設定されたメッセージ</returns>
		public static OpenFileDialogMessage OpenFileDialog( this Messenger messenger, OpenFileDialogMessage message ) {
			return messenger.Raise( message );
		}
	}
}