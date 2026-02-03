using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sylpha.EventListeners.WeakEvents;

namespace Sylpha.Messaging {
	/// <summary>
	/// <see cref="Messenger.Raised"/> イベントを受信するための Weak イベントリスナー
	/// </summary>
	public sealed class MessageListener : WeakEventListener<EventHandler<MessageRaisedEventArgs>, MessageRaisedEventArgs>, IEnumerable<KeyValuePair<string, List<Action<Message>>>>, IDisposable {
		readonly Dictionary<string, List<Action<Message>>> _actionDictionary = [];
		readonly WeakReference<Messenger> _source;

		/// <summary>
		/// <see cref="MessageListener"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="messenger">イベントを受信する対象のメッセンジャー</param>
		/// <exception cref="ArgumentNullException"><paramref name="messenger"/> が <c>null</c> の場合に発生します。</exception>
		public MessageListener( Messenger messenger ) {
			if( messenger == null ) throw new ArgumentNullException( nameof( messenger ) );
			_source = new( messenger );

			var _this = this;
			Initialize(
				h => h,
				h => messenger.Raised += h,
				h => messenger.Raised -= h,
				( sender, e ) => _this.MessageReceived( e )
			);
		}

		void MessageReceived( MessageRaisedEventArgs e ) {
			if( _source.TryGetTarget( out var _ ) ) {
				var message = e.Message;
				var clonedMessage = (Message)message.Clone();
				clonedMessage.Freeze();

				if( e.Message.MessageKey != null ) {
					if( _actionDictionary.TryGetValue( e.Message.MessageKey, out var list ) ) {
						foreach( var action in list ) {
							action( clonedMessage );
						}
					}
				}
				{
					if( _actionDictionary.TryGetValue( string.Empty, out var list ) ) {
						foreach( var action in list ) {
							action( clonedMessage );
						}
					}
				}

				if( message is IRequestMessage requestMessage ) {
					requestMessage.Response = ( (IRequestMessage)clonedMessage ).Response;
				}
			}
		}

		IEnumerator<KeyValuePair<string, List<Action<Message>>>> IEnumerable<KeyValuePair<string, List<Action<Message>>>>.GetEnumerator()
			=> _actionDictionary.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> _actionDictionary.GetEnumerator();

		/// <summary>
		/// <see cref="Action{Message}"/> デリゲートを登録します。
		/// </summary>
		/// <param name="action">登録する <see cref="Action{Message}"/> デリゲート</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c>、または <c>null</c> を含む場合に発生します。</exception>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		public void RegisterAction( params IEnumerable<Action<Message>> action ) => RegisterAction( string.Empty, action );

		/// <summary>
		/// メッセージキーに応答する <see cref="Action{Message}"/> デリゲートを登録します。
		/// </summary>
		/// <param name="messageKey">応答するメッセージキー</param>
		/// <param name="actions">登録する <see cref="Action{Message}"/> デリゲート</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c>、または <c>null</c> を含む場合に発生します。</exception>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		public void RegisterAction( string messageKey, params IEnumerable<Action<Message>> actions ) {
			if( messageKey == null ) throw new ArgumentNullException( nameof( messageKey ) );
			if( actions == null || actions.Contains( null ) ) throw new ArgumentNullException( nameof( actions ) );

			ThrowExceptionIfDisposed();

			if( !_actionDictionary.TryGetValue( messageKey, out var bag ) ) {
				_actionDictionary[messageKey] = bag = [];
			}
			bag.AddRange( actions );
		}

		/// <summary>
		/// <see cref="Action{Message}"/> デリゲートを登録します。
		/// </summary>
		/// <param name="action">登録する <see cref="Action{Message}"/> デリゲート</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c>、または <c>null</c> を含む場合に発生します。</exception>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		public void Add( Action<Message> action ) => this.RegisterAction( string.Empty, action );
		/// <summary>
		/// メッセージキーに応答する <see cref="Action{Message}"/> デリゲートを登録します。
		/// </summary>
		/// <param name="messageKey">応答するメッセージキー</param>
		/// <param name="action">登録する <see cref="Action{Message}"/> デリゲート</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c>、または <c>null</c> を含む場合に発生します。</exception>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		public void Add( string messageKey, Action<Message> action ) => this.RegisterAction( messageKey, action );
		/// <summary>
		/// メッセージキーに応答する <see cref="Action{Message}"/> デリゲートを登録します。
		/// </summary>
		/// <param name="messageKey">応答するメッセージキー</param>
		/// <param name="actions">登録する <see cref="Action{Message}"/> デリゲート</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c>、または <c>null</c> を含む場合に発生します。</exception>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		public void Add( string messageKey, IEnumerable<Action<Message>> actions ) => this.RegisterAction( messageKey, actions );

	}
}