using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sylpha.Messaging {
	/// <summary>
	/// ViewModelで使用するMessengerクラスです。
	/// </summary>
	public class Messenger {
		/// <summary>
		/// 指定されたメッセージを同期的に送信します。
		/// </summary>
		/// <param name="message">メッセージ</param>
		public void Raise( Message message ) {
			if( message == null ) throw new ArgumentNullException( nameof( message ) );
			if( !message.IsFrozen ) message.Freeze();

			Raised?.Invoke( this, new MessageRaisedEventArgs( message ) );
		}

		/// <summary>
		/// 指定された、戻り値情報のあるメッセージを同期的に送信します。
		/// </summary>
		/// <typeparam name="T">戻り値情報のあるメッセージの型</typeparam>
		/// <param name="message">戻り値情報のあるメッセージ</param>
		/// <returns>アクション実行後に、戻り情報を含んだメッセージ</returns>
		public T GetResponse<T>( T message ) where T : Message, IRequest {
			if( message == null ) throw new ArgumentNullException( nameof( message ) );
			if( !message.IsFrozen ) message.Freeze();

			Raised?.Invoke( this, new MessageRaisedEventArgs( message ) );
			return message;
		}

		/// <summary>
		/// メッセージが送信された時に発生するイベントです。
		/// </summary>
		public event EventHandler<MessageRaisedEventArgs>? Raised;

		/// <summary>
		/// 指定されたメッセージを非同期で送信します。
		/// </summary>
		/// <param name="message">メッセージ</param>
		public async Task RaiseAsync( Message message ) {
			if( message == null ) throw new ArgumentNullException( nameof( message ) );

			if( !message.IsFrozen ) message.Freeze();

			await Task.Factory.StartNew( () => Raise( message ) );
		}

		/// <summary>
		/// 指定された、戻り値情報のあるメッセージを非同期で送信します。
		/// </summary>
		/// <typeparam name="T">戻り値情報のあるメッセージの型</typeparam>
		/// <param name="message">戻り値情報のあるメッセージ</param>
		public async Task<T?> GetResponseAsync<T>( T message ) where T : Message, IRequest {
			if( message == null ) throw new ArgumentNullException( nameof( message ) );

			if( !message.IsFrozen ) message.Freeze();

			return await Task<T?>.Factory.StartNew( () => GetResponse( message ) );
		}
	}

	/// <summary>
	/// メッセージ送信時イベント用のイベント引数です。
	/// </summary>
	public class MessageRaisedEventArgs : EventArgs {
		private Message _message;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">Message</param>
		public MessageRaisedEventArgs( Message message ) {
			_message = message ?? throw new ArgumentNullException( nameof( message ) );
		}

		/// <summary>
		/// 送信されたメッセージ
		/// </summary>
		public Message Message {
			get { return _message; }
			set { _message = value ?? throw new ArgumentNullException( nameof( value ) ); }
		}
	}
}