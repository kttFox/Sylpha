using System.Windows;

namespace Sylpha.Messaging {
	/// <summary>
	/// Windowを最大化・最小化・閉じる・通常化させるためのメッセージです。
	/// </summary>
	public class WindowActionMessage : Message {
		#region ActionProperty
		/// <summary>
		/// Windowが遷移すべき状態を表すWindowAction列挙体を指定、または取得します。
		/// </summary>
		public WindowAction? Action {
			get { return (WindowAction?)( GetValue( ActionProperty ) ); }
			set { SetValue( ActionProperty, value ); }
		}

		public static readonly DependencyProperty ActionProperty =
			DependencyProperty.Register( nameof( Action ), typeof( WindowAction ), typeof( WindowActionMessage ), new PropertyMetadata( null ) );
		#endregion

		public WindowActionMessage() { }

		/// <summary>
		/// メッセージキーを指定して新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public WindowActionMessage( string? messageKey ) : base( messageKey ) { }

		/// <summary>
		/// メッセージキーとWindowが遷移すべき状態を定義して、新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="action">Windowが遷移すべき状態を表すWindowAction列挙体</param>
		/// <param name="messageKey">メッセージキー</param>
		public WindowActionMessage( WindowAction action, string? messageKey = null ) : this( messageKey ) {
			Action = action;
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new WindowActionMessage( MessageKey );
		}
	}
}