using System.Windows;
using JetBrains.Annotations;


namespace Sylpha.Messaging {
	/// <summary>
	/// 戻り値のある相互作用メッセージの抽象基底クラスです。
	/// </summary>
	public abstract class RequestMessage : Message {
		internal RequestMessage() { }

		internal RequestMessage( string? messageKey ) : base( messageKey ) { }

		internal object? Response { get; set; }
	}

	/// <summary>
	/// 戻り値のある相互作用メッセージの基底クラスです。
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	[PublicAPI]
	public class RequestMessage<TResult> : RequestMessage {
		public RequestMessage() { }

		/// <summary>
		/// メッセージキーを使用して、戻り値のある新しい相互作用メッセージのインスタンスを生成します
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public RequestMessage( string? messageKey ) : base( messageKey ) { }

		/// <summary>
		/// 戻り値情報
		/// </summary>
		public new TResult? Response {
			get { return (TResult?)base.Response; }
			set { base.Response = value; }
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new RequestMessage<TResult>();
		}
	}
}