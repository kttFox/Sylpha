using System.Windows;
using JetBrains.Annotations;

namespace Sylpha.Messaging {
	/// <summary>
	/// 戻り値のあるメッセージの共通インターフェイス
	/// </summary>
	public interface IRequestMessage {
		object? Response { get; set; }
	}

	/// <summary>
	/// 戻り値のあるメッセージ
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	[PublicAPI]
	public class RequestMessage<TResult> : Message, IRequestMessage {
		/// <summary>
		/// 戻り値のある新しいメッセージのインスタンスを生成します
		/// </summary>
		public RequestMessage() { }

		/// <summary>
		/// メッセージキーを使用して、戻り値のある新しいメッセージのインスタンスを生成します
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public RequestMessage( string? messageKey ) : base( messageKey ) { }

		/// <summary>
		/// 戻り値
		/// </summary>
		public TResult? Response { get; set; }

		object? IRequestMessage.Response {
			get => this.Response;
			set => this.Response = (TResult?)value;
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() => new RequestMessage<TResult>();
	}

	/// <summary>
	/// 値と戻り値のあるメッセージの抽象基底クラスです。
	/// </summary>
	[PublicAPI]
	public class RequestMessage<TValue, TResult> : Message<TValue>, IRequestMessage {
		/// <summary>
		/// 値と戻り値のある新しいメッセージのインスタンスを生成します。
		/// </summary>
		public RequestMessage() { }

		/// <summary>
		/// メッセージキーを使用して、値と戻り値のある新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public RequestMessage( string? messageKey ) : base( messageKey ) { }

		/// <summary>
		/// メッセージキーを使用して、値と戻り値のある新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="value">値</param>
		/// <param name="messageKey">メッセージキー</param>
		public RequestMessage( TValue value, string? messageKey = null ) : base( value, messageKey ) { }

		/// <summary>
		/// 戻り値
		/// </summary>
		public TResult? Response { get; set; }

		object? IRequestMessage.Response {
			get => this.Response;
			set => this.Response = (TResult?)value;
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() => new RequestMessage<TValue, TResult>();
	}
}