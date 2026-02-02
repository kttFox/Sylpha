using System.Windows;

namespace Sylpha.Messaging {
	/// <summary>
	/// フォルダ選択ダイアログ用のメッセージです。
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

		public static readonly DependencyProperty MultiselectProperty =
			DependencyProperty.Register( nameof( Multiselect ), typeof( bool ), typeof( CommonOpenFileDialogMessage ), new UIPropertyMetadata( false ) );
		#endregion


		/// <summary>
		/// 新しいメッセージのインスタンスを生成します。
		/// </summary>
		public CommonOpenFileDialogMessage() { }

		/// <summary>
		/// メッセージキーを指定して新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public CommonOpenFileDialogMessage( string? messageKey ) : base( messageKey ) { }


		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		protected override Freezable CreateInstanceCore() => new CommonOpenFileDialogMessage();
	}
}