using System.Windows;

namespace Sylpha.Messaging {
	/// <summary>
	/// 戻り値のあるメッセージの共通インターフェイス
	/// </summary>
	public interface IRequestMessage {
		/// <summary>
		/// 戻り値を取得または設定します。
		/// </summary>
		object? Response { get; set; }
	}

	/// <summary>
	/// 戻り値のあるメッセージの共通インターフェイス
	/// </summary>
	public interface IRequestMessage<TResult> {
		/// <summary>
		/// 戻り値を取得または設定します。
		/// </summary>
		TResult? Response { get; set; }
	}

	/// <summary>
	/// 戻り値のあるメッセージの基底クラス
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	public class RequestMessage<TResult> : Message, IRequestMessage<TResult>, IRequestMessage {
		/// <summary>
		/// <see cref="RequestMessage{TResult}"/> の新しいインスタンスを初期化します。
		/// </summary>
		public RequestMessage() { }

		/// <summary>
		/// メッセージキーを指定して、<see cref="RequestMessage{TResult}"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public RequestMessage( string? messageKey ) : base( messageKey ) { }

		/// <inheritdoc />
		public TResult? Response { get; set; }

		/// <inheritdoc />
		object? IRequestMessage.Response {
			get => this.Response;
			set => this.Response = (TResult?)value;
		}

		/// <inheritdoc />
		protected override Freezable CreateInstanceCore() => new RequestMessage<TResult>();
	}

	/// <summary>
	/// 値と戻り値のあるメッセージの基底クラス
	/// </summary>
	public class RequestMessage<TValue, TResult> : Message<TValue>, IRequestMessage<TResult>, IRequestMessage {
		/// <summary>
		/// <see cref="RequestMessage{TValue,TResult}"/> の新しいインスタンスを初期化します。
		/// </summary>
		public RequestMessage() { }

		/// <summary>
		/// メッセージキーを指定して、<see cref="RequestMessage{TValue,TResult}"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public RequestMessage( string? messageKey ) : base( messageKey ) { }

		/// <summary>
		/// 値とメッセージキーを指定して、<see cref="RequestMessage{TValue,TResult}"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="value">値</param>
		/// <param name="messageKey">メッセージキー</param>
		public RequestMessage( TValue value, string? messageKey = null ) : base( value, messageKey ) { }

		/// <inheritdoc />
		public TResult? Response { get; set; }

		/// <inheritdoc />
		object? IRequestMessage.Response {
			get => this.Response;
			set => this.Response = (TResult?)value;
		}

		/// <inheritdoc />
		protected override Freezable CreateInstanceCore() => new RequestMessage<TValue, TResult>();
	}
}