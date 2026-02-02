using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sylpha.Messaging {
	/// <summary>
	/// ViewModelで使用するMessengerクラスです。
	/// </summary>
	public class Messenger {
		/// <summary>
		/// メッセージが送信された時に発生するイベントです。
		/// </summary>
		public event EventHandler<MessageRaisedEventArgs>? Raised;


		/// <summary>
		/// 指定されたメッセージを送信します。
		/// </summary>
		/// <typeparam name="T">メッセージの型</typeparam>
		/// <param name="message">メッセージ</param>
		/// <returns>アクション実行後のメッセージ(引数と同じ)</returns>
		public T Raise<T>( T message ) where T : Message {
			if( message == null ) throw new ArgumentNullException( nameof( message ) );
			if( !message.IsFrozen ) message.Freeze();

			Raised?.Invoke( this, new MessageRaisedEventArgs( message ) );

			return message;
		}
	}

	/// <summary>
	/// メッセージ送信時イベント用のイベント引数です。
	/// </summary>
	/// <param name="message">Message</param>
	public class MessageRaisedEventArgs( Message message ) : EventArgs {

		/// <summary>
		/// 送信されたメッセージ
		/// </summary>
		public Message Message { get; } = message ?? throw new ArgumentNullException( nameof( message ) );
	}
}