using System.Windows;
using JetBrains.Annotations;

namespace Sylpha.Messaging {
	/// <summary>
	/// メッセージの基底クラスです。<br />
	/// Viewからのアクション実行後、戻り値情報が必要ないメッセージを作成する場合はこのクラスを継承してメッセージを作成します。
	/// </summary>
	[PublicAPI]
	public class Message : Freezable {

		#region Register MessageKey
		/// <summary>
		/// メッセージキーを指定、または取得します。
		/// </summary>
		public string? MessageKey {
			get => (string?)GetValue( MessageKeyProperty );
			set => SetValue( MessageKeyProperty, value );
		}

		public static readonly DependencyProperty MessageKeyProperty =
			DependencyProperty.Register( nameof( MessageKey ), typeof( string ), typeof( Message ), new PropertyMetadata( default( string? ) ) );
		#endregion

		public Message() { }

		/// <summary>
		/// メッセージキーを指定して新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public Message( string? messageKey ) {
			MessageKey = messageKey;
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new Message( MessageKey );
		}
	}
}