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
}