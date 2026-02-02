using System.Windows;
using JetBrains.Annotations;

namespace Sylpha.Messaging {
	/// <summary>
	/// メッセージの基底クラスです。<br />
	/// 戻り値情報が必要ないメッセージを作成する場合はこのクラスを継承してメッセージを作成します。
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
			DependencyProperty.Register( nameof( MessageKey ), typeof( string ), typeof( Message ), new PropertyMetadata( null ) );
		#endregion

		/// <summary>
		/// 新しいメッセージのインスタンスを生成します。
		/// </summary>
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
		protected override Freezable CreateInstanceCore() => new Message();
	}

	/// <summary>
	/// 値のあるメッセージの基底クラスです。<br />
	/// 戻り値情報が必要ないメッセージを作成する場合はこのクラスを継承してメッセージを作成します。
	/// </summary>
	[PublicAPI]
	public class Message<TValue> : Message {

		#region Register ValueProperty
		/// <summary>
		/// 値を指定、または取得します。
		/// </summary>
		public TValue Value {
			get { return (TValue)GetValue( ValueProperty ); }
			set { SetValue( ValueProperty, value ); }
		}

		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register( nameof( Value ), typeof( TValue ), typeof( Message<TValue> ), new PropertyMetadata( default( TValue ) ) );
		#endregion

		public Message() { }

		public Message( string? messageKey ) {
			MessageKey = messageKey;
		}

		public Message( TValue value, string? messageKey = null ) : base( messageKey ) {
			Value = value;
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		protected override Freezable CreateInstanceCore() => new Message<TValue>();
	}
}