using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sylpha.Messaging {
	/// <summary>
	/// ViewModelで使用するMessengerクラスです。
	/// </summary>
	public class InteractionMessenger {
		/// <summary>
		/// 指定された相互作用メッセージを同期的に送信します。
		/// </summary>
		/// <param name="message">相互作用メッセージ</param>
		public void Raise( InteractionMessage message ) {
			if( message == null ) throw new ArgumentNullException( nameof( message ) );

			var threadSafeHandler = Interlocked.CompareExchange( ref Raised, null, null );
			if( threadSafeHandler == null ) return;

			if( !message.IsFrozen ) message.Freeze();

			threadSafeHandler( this, new InteractionMessageRaisedEventArgs( message ) );
		}

		/// <summary>
		/// 指定された、戻り値情報のある相互作用メッセージを同期的に送信します。
		/// </summary>
		/// <typeparam name="T">戻り値情報のある相互作用メッセージの型</typeparam>
		/// <param name="message">戻り値情報のある相互作用メッセージ</param>
		/// <returns>アクション実行後に、戻り情報を含んだ相互作用メッセージ</returns>
		public T? GetResponse<T>( T message ) where T : ResponsiveInteractionMessage {
			if( message == null ) throw new ArgumentNullException( nameof( message ) );

			var threadSafeHandler = Interlocked.CompareExchange( ref Raised, null, null );
			if( threadSafeHandler == null ) return null;

			if( !message.IsFrozen ) message.Freeze();

			threadSafeHandler( this, new InteractionMessageRaisedEventArgs( message ) );
			return message;
		}

		/// <summary>
		/// 相互作用メッセージが送信された時に発生するイベントです。
		/// </summary>
		public event EventHandler<InteractionMessageRaisedEventArgs>? Raised;

		/// <summary>
		/// 指定された相互作用メッセージを非同期で送信します。
		/// </summary>
		/// <param name="message">相互作用メッセージ</param>
		public async Task RaiseAsync( InteractionMessage message ) {
			if( message == null ) throw new ArgumentNullException( nameof( message ) );

			if( !message.IsFrozen ) message.Freeze();

			await Task.Factory.StartNew( () => Raise( message ) );
		}

		/// <summary>
		/// 指定された、戻り値情報のある相互作用メッセージを非同期で送信します。
		/// </summary>
		/// <typeparam name="T">戻り値情報のある相互作用メッセージの型</typeparam>
		/// <param name="message">戻り値情報のある相互作用メッセージ</param>
		public async Task<T?> GetResponseAsync<T>( T message ) where T : ResponsiveInteractionMessage {
			if( message == null ) throw new ArgumentNullException( nameof( message ) );

			if( !message.IsFrozen ) message.Freeze();

			return await Task<T?>.Factory.StartNew( () => GetResponse( message ) );
		}
	}

	/// <summary>
	/// 相互作用メッセージ送信時イベント用のイベント引数です。
	/// </summary>
	public class InteractionMessageRaisedEventArgs : EventArgs {
		private InteractionMessage _message;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">InteractionMessage</param>
		public InteractionMessageRaisedEventArgs( InteractionMessage message ) {
			_message = message ?? throw new ArgumentNullException( nameof( message ) );
		}

		/// <summary>
		/// 送信されたメッセージ
		/// </summary>
		public InteractionMessage Message {
			get { return _message; }
			set { _message = value ?? throw new ArgumentNullException( nameof( value ) ); }
		}
	}
}