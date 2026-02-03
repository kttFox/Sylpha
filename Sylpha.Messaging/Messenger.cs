using System;

namespace Sylpha.Messaging {
	
	/// <summary>
	/// メッセージングシステムの中心となるクラス<br />
	/// メッセージを送信し、リスナーに通知します。
	/// </summary>
	public class Messenger {
		/// <summary>
		/// メッセージが送信されたときに発生するイベント
		/// </summary>
		public event EventHandler<MessageRaisedEventArgs>? Raised;

		/// <summary>
		/// 指定されたメッセージを送信します。
		/// </summary>
		/// <typeparam name="T">メッセージの型</typeparam>
		/// <param name="message">送信するメッセージ</param>
		/// <returns>送信されたメッセージ（引数と同じインスタンス）</returns>
		/// <exception cref="ArgumentNullException"><paramref name="message"/> が <c>null</c> の場合に発生します。</exception>
		public T Raise<T>( T message ) where T : Message {
			if( message == null ) throw new ArgumentNullException( nameof( message ) );
			if( !message.IsFrozen ) message.Freeze();

			Raised?.Invoke( this, new MessageRaisedEventArgs( message ) );

			return message;
		}
	}

	/// <summary>
	/// メッセージ送信時のイベント引数
	/// </summary>
	/// <param name="message">送信されたメッセージ</param>
	public class MessageRaisedEventArgs( Message message ) : EventArgs {
		/// <summary>
		/// 送信されたメッセージ
		/// </summary>
		public Message Message { get; } = message ?? throw new ArgumentNullException( nameof( message ) );
	}
}