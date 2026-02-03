using System.Windows;

namespace Sylpha.Messaging {
	/// <summary>
	/// メッセージの基底クラス
	/// </summary>
	public class Message : Freezable {
		#region Register MessageKey
		/// <summary>
		/// メッセージキーを指定、または取得します。
		/// </summary>
		public string? MessageKey {
			get => (string?)GetValue( MessageKeyProperty );
			set => SetValue( MessageKeyProperty, value );
		}
		
		/// <summary>
		/// <see cref="MessageKey"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty MessageKeyProperty =
			DependencyProperty.Register( nameof( MessageKey ), typeof( string ), typeof( Message ), new PropertyMetadata( null ) );
		#endregion

		/// <summary>
		/// <see cref="Message"/> の新しいインスタンスを初期化します。
		/// </summary>
		public Message() { }

		/// <summary>
		/// メッセージキーを指定して、<see cref="Message"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public Message( string? messageKey ) {
			MessageKey = messageKey;
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。<see cref="Freezable"/> オブジェクトとして必要な実装です。<br />
		/// 通常、このメソッドは自身の新しいインスタンスを返すように実装します。
		/// </summary>
		protected override Freezable CreateInstanceCore() => new Message();
	}

	/// <summary>
	/// 値のあるメッセージの基底クラス
	/// </summary>
	/// <typeparam name="TValue">メッセージが保持する値の型</typeparam>
	public class Message<TValue> : Message {
		#region Register ValueProperty
		/// <summary>
		/// 値を指定、または取得します。
		/// </summary>
		public TValue Value {
			get => (TValue)GetValue( ValueProperty );
			set => SetValue( ValueProperty, value );
		}

		/// <summary>
		/// <see cref="Value"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register( nameof( Value ), typeof( TValue ), typeof( Message<TValue> ), new PropertyMetadata( default( TValue ) ) );
		#endregion

		/// <summary>
		/// <see cref="Message{TValue}"/> の新しいインスタンスを初期化します。
		/// </summary>
		public Message() { }

		/// <summary>
		/// メッセージキーを指定して、<see cref="Message{TValue}"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public Message( string? messageKey ) {
			MessageKey = messageKey;
		}

		/// <summary>
		/// 値とメッセージキーを指定して、<see cref="Message{TValue}"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="value">メッセージの値</param>
		/// <param name="messageKey">メッセージキー</param>
		public Message( TValue value, string? messageKey = null ) : base( messageKey ) {
			Value = value;
		}

		/// <inheritdoc />
		protected override Freezable CreateInstanceCore() => new Message<TValue>();
	}
}