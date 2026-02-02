using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using Sylpha.EventListeners.WeakEvents;

namespace Sylpha.Messaging {
	public sealed class MessageListener : IDisposable, IEnumerable<KeyValuePair<string, ConcurrentBag<Action<Message>>>> {
		private readonly ConcurrentDictionary<string, ConcurrentBag<Action<Message>>> _actionDictionary = new();

		private readonly WeakEventListener<EventHandler<MessageRaisedEventArgs>, MessageRaisedEventArgs> _listener;

		private readonly WeakReference<Messenger> _source;
		private Dispatcher _dispatcher;

		private bool _disposed;

		public MessageListener( Messenger messenger ) {
			if( messenger == null ) throw new ArgumentNullException( nameof( messenger ) );

			_dispatcher = Dispatcher.CurrentDispatcher;
			_source = new( messenger );
			_listener = new(
					h => h,
					h => messenger.Raised += h,
					h => messenger.Raised -= h,
					MessageReceived
				);
		}

		public MessageListener( Messenger messenger, string? messageKey, Action<Message> action ) : this( messenger ) {
			if( action == null ) throw new ArgumentNullException( nameof( action ) );
			messageKey ??= string.Empty;

			RegisterAction( messageKey, action );
		}

		public MessageListener( Messenger messenger, Action<Message> action ) : this( messenger, null, action ) { }

		public Dispatcher Dispatcher {
			get { return _dispatcher; }
			set { _dispatcher = value ?? throw new ArgumentNullException( nameof( value ) ); }
		}

		public void Dispose() {
			Dispose( true );

			// ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
			GC.SuppressFinalize( this );
		}

		IEnumerator<KeyValuePair<string, ConcurrentBag<Action<Message>>>> IEnumerable<KeyValuePair<string, ConcurrentBag<Action<Message>>>>.GetEnumerator() {
			ThrowExceptionIfDisposed();
			return _actionDictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			ThrowExceptionIfDisposed();
			return _actionDictionary.GetEnumerator();
		}

		public void RegisterAction( Action<Message> action ) {
			if( action == null ) throw new ArgumentNullException( nameof( action ) );

			ThrowExceptionIfDisposed();
			var dic = _actionDictionary.GetOrAdd( string.Empty, _ => [] ) ?? throw new InvalidOperationException();

			dic.Add( action );
		}

		public void RegisterAction( string messageKey, Action<Message> action ) {
			if( messageKey == null ) throw new ArgumentNullException( nameof( messageKey ) );
			if( action == null ) throw new ArgumentNullException( nameof( action ) );

			ThrowExceptionIfDisposed();
			var dic = _actionDictionary.GetOrAdd( messageKey, _ => [] ) ?? throw new InvalidOperationException();

			dic.Add( action );
		}

		private void MessageReceived( object? sender, MessageRaisedEventArgs e ) {
			if( e == null ) throw new ArgumentNullException( nameof( e ) );
			if( _disposed ) return;

			var message = e.Message;
			var clonedMessage = (Message)message.Clone();

			clonedMessage.Freeze();

			DoActionOnDispatcher( () => { GetValue( e, clonedMessage ); } );


			if( message is RequestMessage responsiveMessage ) {
				object? response = ( (RequestMessage)clonedMessage ).Response;
				if( response != null ) {
					responsiveMessage.Response = response;
				}
			}
		}

		private void GetValue( MessageRaisedEventArgs e, Message cloneMessage ) {
			if( e == null ) throw new ArgumentNullException( nameof( e ) );

			var result = _source.TryGetTarget( out _ );

			if( !result ) return;

			if( e.Message.MessageKey != null ) {
				_actionDictionary.TryGetValue( e.Message.MessageKey, out var list );

				if( list != null ) {
					foreach( var action in list ) {
						action( cloneMessage );
					}
				}
			}

			_actionDictionary.TryGetValue( string.Empty, out var allList );
			if( allList == null ) return;

			foreach( var action in allList ) {
				action( cloneMessage );
			}
		}

		private void DoActionOnDispatcher( Action action ) {
			if( action == null ) throw new ArgumentNullException( nameof( action ) );

			if( Dispatcher.CheckAccess() ) {
				action();
			} else {
				Dispatcher.Invoke( action );
			}
		}

		public void Add( Action<Message> action ) {
			if( action == null ) throw new ArgumentNullException( nameof( action ) );

			RegisterAction( action );
		}

		public void Add( string messageKey, Action<Message> action ) {
			if( messageKey == null ) throw new ArgumentNullException( nameof( messageKey ) );
			if( action == null ) throw new ArgumentNullException( nameof( action ) );

			RegisterAction( messageKey, action );
		}

		public void Add( string messageKey, params Action<Message>[] actions ) {
			if( messageKey == null ) throw new ArgumentNullException( nameof( messageKey ) );
			if( actions == null ) throw new ArgumentNullException( nameof( actions ) );

			foreach( var action in actions.Where( a => a != null ) ) {
				RegisterAction( messageKey, action );
			}
		}

		private void ThrowExceptionIfDisposed() {
			if( _disposed ) throw new ObjectDisposedException( "EventListener" );
		}

		private void Dispose( bool disposing ) {
			if( _disposed ) return;

			if( disposing ) _listener.Dispose();
			_disposed = true;
		}
	}
}