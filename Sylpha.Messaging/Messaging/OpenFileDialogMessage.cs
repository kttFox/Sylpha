using System.Windows;

namespace Sylpha.Messaging {
	/// <summary>
	/// ファイルを開く アクション用のメッセージ
	/// </summary>
	public class OpenFileDialogMessage : FileDialogMessage {
		#region Register MultiSelect
		/// <summary>
		/// 複数ファイルを選択可能かを取得、または設定します。
		/// </summary>
		public bool MultiSelect {
			get => (bool)GetValue( MultiSelectProperty );
			set => SetValue( MultiSelectProperty, value );
		}

		/// <summary>
		/// <see cref="MultiSelect"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty MultiSelectProperty =
			DependencyProperty.Register( nameof( MultiSelect ), typeof( bool ), typeof( OpenFileDialogMessage ), new PropertyMetadata( false ) );
		#endregion


		/// <summary>
		/// <see cref="OpenFileDialogMessage"/> の新しいインスタンスを初期化します。
		/// </summary>
		public OpenFileDialogMessage() { }

		/// <summary>
		/// メッセージキーを指定して、<see cref="OpenFileDialogMessage"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public OpenFileDialogMessage( string? messageKey ) : base( messageKey ) { }

		/// <inheritdoc />
		protected override Freezable CreateInstanceCore() => new OpenFileDialogMessage();

	}

	/// <summary>
	/// <see cref="OpenFileDialogMessage"/> 用の拡張メソッドを提供します。
	/// </summary>
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