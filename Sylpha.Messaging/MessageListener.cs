using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Sylpha.EventListeners.WeakEvents;

namespace Sylpha.Messaging {
	public sealed class MessageListener : IDisposable, IEnumerable<KeyValuePair<string, ConcurrentBag<Action<Message>>>> {
		private readonly ConcurrentDictionary<string, ConcurrentBag<Action<Message>>> _actionDictionary = [];

		private readonly WeakEventListener<EventHandler<MessageRaisedEventArgs>, MessageRaisedEventArgs> _listener;

		public MessageListener( Messenger messenger ) {
			if( messenger == null ) throw new ArgumentNullException( nameof( messenger ) );

			_listener = new(
					h => h,
					h => messenger.Raised += h,
					h => messenger.Raised -= h,
					MessageReceived
				);
		}

		public MessageListener( Messenger messenger, string? messageKey, Action<Message> action ) : this( messenger ) {
			if( action == null ) throw new ArgumentNullException( nameof( action ) );

			RegisterAction( messageKey ?? string.Empty, action );
		}

		public MessageListener( Messenger messenger, Action<Message> action ) : this( messenger, null, action ) { }

		IEnumerator<KeyValuePair<string, ConcurrentBag<Action<Message>>>> IEnumerable<KeyValuePair<string, ConcurrentBag<Action<Message>>>>.GetEnumerator() {
			ThrowExceptionIfDisposed();
			return _actionDictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			ThrowExceptionIfDisposed();
			return _actionDictionary.GetEnumerator();
		}

		public void RegisterAction( Action<Message> action ) => RegisterAction( string.Empty, action );

		public void RegisterAction( string messageKey, Action<Message> action ) {
			if( messageKey == null ) throw new ArgumentNullException( nameof( messageKey ) );
			if( action == null ) throw new ArgumentNullException( nameof( action ) );

			ThrowExceptionIfDisposed();

			_actionDictionary.GetOrAdd( messageKey, x => [] ).Add( action );
		}

		private void MessageReceived( object? sender, MessageRaisedEventArgs e ) {
			if( e == null ) throw new ArgumentNullException( nameof( e ) );
			if( _disposed ) return;

			var message = e.Message;
			var clonedMessage = (Message)message.Clone();
			clonedMessage.Freeze();

			GetValue( e, clonedMessage );

			if( message is IRequestMessage requestMessage ) {
				object? response = ( (IRequestMessage)clonedMessage ).Response;
				if( response != null ) {
					requestMessage.Response = response;
				}
			}
		}

		private void GetValue( MessageRaisedEventArgs e, Message cloneMessage ) {
			if( e == null ) throw new ArgumentNullException( nameof( e ) );

			if( e.Message.MessageKey != null ) {
				if( _actionDictionary.TryGetValue( e.Message.MessageKey, out var list ) ) {
					foreach( var action in list ) {
						action( cloneMessage );
					}
				}
			}

			if( _actionDictionary.TryGetValue( string.Empty, out var allList ) ) {
				foreach( var action in allList ) {
					action( cloneMessage );
				}
			}
		}

		public void Add( Action<Message> action ) => RegisterAction( string.Empty, action );

		public void Add( string messageKey, Action<Message> action ) => RegisterAction( messageKey, action );

		public void Add( string messageKey, params Action<Message>[] actions ) {
			if( messageKey == null ) throw new ArgumentNullException( nameof( messageKey ) );
			if( actions == null ) throw new ArgumentNullException( nameof( actions ) );

			foreach( var action in actions ) {
				RegisterAction( messageKey, action );
			}
		}

		#region Dispose
		public void Dispose() {
			Dispose( true );
		}

		private bool _disposed;

		private void Dispose( bool disposing ) {
			if( _disposed ) return;

			if( disposing ) _listener.Dispose();
			_disposed = true;
		}

		private void ThrowExceptionIfDisposed() {
			if( _disposed ) throw new ObjectDisposedException( "EventListener" );
		}
		#endregion
	}
}