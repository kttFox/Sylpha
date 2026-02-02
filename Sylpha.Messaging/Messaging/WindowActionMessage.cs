using System.Windows;

namespace Sylpha.Messaging {
	/// <summary>
	/// Windowを最大化、最小化、閉じる などを行うためのメッセージです。
	/// </summary>
	public class WindowActionMessage : Message {
		#region WindowActionProperty
		/// <summary>
		/// Windowが遷移すべき状態を表す<see cref="WindowActionMode"/>列挙体を指定、または取得します。
		/// </summary>
		public WindowActionMode? WindowAction {
			get { return (WindowActionMode?)( GetValue( WindowActionProperty ) ); }
			set { SetValue( WindowActionProperty, value ); }
		}

		public static readonly DependencyProperty WindowActionProperty =
			DependencyProperty.Register( nameof( WindowAction ), typeof( WindowActionMode ), typeof( WindowActionMessage ), new PropertyMetadata( null ) );
		#endregion

		/// <summary>
		/// 新しいメッセージのインスタンスを生成します。
		/// </summary>
		public WindowActionMessage() { }

		/// <summary>
		/// メッセージキーとWindowが遷移すべき状態を定義して、新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="windowAction">Windowが遷移すべき状態を表すWindowAction列挙体</param>
		/// <param name="messageKey">メッセージキー</param>
		public WindowActionMessage( WindowActionMode windowAction, string? messageKey = null ) : base( messageKey ) {
			this.WindowAction = windowAction;
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		protected override Freezable CreateInstanceCore() => new WindowActionMessage();
	}

	public static class WindowActionMessageExtensions {
		/// <summary>
		/// Windowを最大化、最小化、閉じる などを行うためのメッセージを送信します。
		/// </summary>
		/// <param name="messenger">送信先</param>
		/// <param name="message">送信するメッセージ</param>
		public static void WindowAction( this Messenger messenger, WindowActionMessage message ) {
			messenger.Raise( message );
		}
	}
}