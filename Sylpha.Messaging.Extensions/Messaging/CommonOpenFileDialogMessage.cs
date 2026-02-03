using System.Windows;

namespace Sylpha.Messaging {
	/// <summary>
	/// フォルダ選択ダイアログ用のメッセージ
	/// </summary>
	public class CommonOpenFileDialogMessage : CommonItemDialogMessage {
		#region Register Multiselect
		/// <summary>
		/// 複数選択が有効かどうかを取得または設定します。
		/// </summary>
		public bool Multiselect {
			get => (bool)GetValue( MultiselectProperty );
			set => SetValue( MultiselectProperty, value );
		}

		/// <summary>
		/// <see cref="Multiselect"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty MultiselectProperty =
			DependencyProperty.Register( nameof( Multiselect ), typeof( bool ), typeof( CommonOpenFileDialogMessage ), new UIPropertyMetadata( false ) );
		#endregion


		/// <summary>
		/// <see cref="CommonOpenFileDialogMessage"/> の新しいインスタンスを初期化します。
		/// </summary>
		public CommonOpenFileDialogMessage() { }

		/// <summary>
		/// メッセージキーを指定して、<see cref="CommonOpenFileDialogMessage"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public CommonOpenFileDialogMessage( string? messageKey ) : base( messageKey ) { }


		/// <inheritdoc />
		protected override Freezable CreateInstanceCore() => new CommonOpenFileDialogMessage();
	}

	/// <summary>
	/// <see cref="CommonOpenFileDialogMessage"/> 用の拡張メソッドを提供します。
	/// </summary>
	public static class CommonOpenFileDialogMessageExtensions {
		/// <summary>
		/// フォルダ選択ダイアログを表示するメッセージを送信し、結果を取得します。
		/// </summary>
		/// <param name="messenger">送信先</param>
		/// <param name="message">送信するメッセージ</param>
		/// <returns>結果が設定されたメッセージ</returns>
		public static CommonOpenFileDialogMessage CommonOpenFileDialog( this Messenger messenger, CommonOpenFileDialogMessage message ) {
			return messenger.Raise( message );
		}
	}
}