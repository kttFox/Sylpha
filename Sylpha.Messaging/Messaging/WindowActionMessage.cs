using System.Windows;

namespace Sylpha.Messaging {
	/// <summary>
	/// ウインドウを最大化、最小化、閉じる などを行うためのメッセージ
	/// </summary>
	public class WindowActionMessage : Message {
		#region WindowActionProperty
		/// <summary>
		/// ウィンドウに対して実行するアクションを取得または設定します。
		/// </summary>
		public WindowActionMode? WindowAction {
			get => (WindowActionMode?)GetValue( WindowActionProperty );
			set => SetValue( WindowActionProperty, value );
		}

		/// <summary>
		/// <see cref="WindowAction"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty WindowActionProperty =
			DependencyProperty.Register( nameof( WindowAction ), typeof( WindowActionMode ), typeof( WindowActionMessage ), new PropertyMetadata( null ) );
		#endregion

		/// <summary>
		/// <see cref="WindowActionMessage"/> の新しいインスタンスを初期化します。
		/// </summary>
		public WindowActionMessage() { }

		/// <summary>
		/// ィンドウに対して実行するアクションとメッセージキーを指定して、<see cref="WindowActionMessage"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="windowAction">Windowが遷移すべき状態を表すWindowAction列挙体</param>
		/// <param name="messageKey">メッセージキー</param>
		public WindowActionMessage( WindowActionMode windowAction, string? messageKey = null ) : base( messageKey ) {
			this.WindowAction = windowAction;
		}

		/// <inheritdoc />
		protected override Freezable CreateInstanceCore() => new WindowActionMessage();
	}

	/// <summary>
	/// ウィンドウに対して実行するアクションを指定します。
	/// </summary>
	public enum WindowActionMode {
		/// <summary>
		/// ウィンドウを閉じます。
		/// </summary>
		Close,
		/// <summary>
		/// ウィンドウを最大化します。
		/// </summary>
		Maximize,
		/// <summary>
		/// ウィンドウを最小化します。
		/// </summary>
		Minimize,
		/// <summary>
		/// ウィンドウを通常状態にします。
		/// </summary>
		Normal,
		/// <summary>
		/// ウィンドウをアクティブにします。
		/// </summary>
		Active,
		/// <summary>
		/// ウィンドウを非表示にします。
		/// </summary>
		Hide,

		/// <summary>
		/// ウィンドウのDialogResultをTrueにします。
		/// </summary>
		ResultOK,
		/// <summary>
		/// ウィンドウのDialogResultをFalseにします。
		/// </summary>
		ResultCancel,
	}

	/// <summary>
	/// <see cref="WindowActionMessage"/> 用の拡張メソッドを提供します。
	/// </summary>
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