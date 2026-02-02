using System;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using Sylpha.EventListeners.WeakEvents;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// ViewModelからのメッセージを受信し、アクションを実行します。
	/// </summary>
	public class MessageTrigger : TriggerBase<FrameworkElement>, IDisposable {
		#region Register MessengerProperty
		/// <summary>
		/// ViewModelのMessengerを指定、または取得します。
		/// </summary>
		public Messenger? Messenger {
			get { return (Messenger?)GetValue( MessengerProperty ); }
			set { SetValue( MessengerProperty, value ); }
		}

		public static readonly DependencyProperty MessengerProperty =
					DependencyProperty.Register( nameof( Messenger ), typeof( Messenger ), typeof( MessageTrigger ), new PropertyMetadata( null, MessengerChanged ) );
		
		private static void MessengerChanged( DependencyObject d, DependencyPropertyChangedEventArgs e ) {
			if( d == null ) throw new ArgumentNullException( nameof( d ) );
			var thisReference = (MessageTrigger)d;

			if( e.OldValue == e.NewValue ) return;

			if( e.OldValue != null ) thisReference._listener?.Dispose();

			if( e.NewValue == null ) return;
			var newMessenger = (Messenger)e.NewValue;

			thisReference._listener =
				new WeakEventListener<EventHandler<MessageRaisedEventArgs>, MessageRaisedEventArgs>(
					h => h,
					h => newMessenger.Raised += h,
					h => newMessenger.Raised -= h,
					thisReference.MessageReceived );
		}

		private void MessageReceived( object? sender, MessageRaisedEventArgs e ) {
			if( e == null ) throw new ArgumentNullException( nameof( e ) );

			var message = e.Message;

			var cloneMessage = (Message)message.Clone();
			cloneMessage.Freeze();

			var checkResult = false;

			DoActionOnDispatcher( () => {
				if( MessageKey != cloneMessage.MessageKey ) {
					return;
				}

				checkResult = true;
			} );

			if( !checkResult ) return;

			DoActionOnDispatcher( () => InvokeActions( cloneMessage ) );


			if( message is IRequest requestMessage ) {
				requestMessage.Response = ( (IRequest)cloneMessage ).Response;
			}
		}

		private void DoActionOnDispatcher( Action action ) {
			if( Dispatcher == null ) throw new InvalidOperationException( "Dispatcher is null." );

			if( Dispatcher.CheckAccess() ) {
				action();
			} else {
				Dispatcher.Invoke( action );
			}
		}

		#endregion

		private WeakEventListener<EventHandler<MessageRaisedEventArgs>, MessageRaisedEventArgs>? _listener;

		/// <summary>
		/// このトリガーが反応するメッセージのメッセージキーを指定、または取得します。<br />
		/// このプロパティが指定されていない場合、このトリガーは全てのメッセージに反応します。
		/// </summary>
		public string? MessageKey { get; set; }


		protected override void OnAttached() {
			base.OnAttached();
		}

		protected override void OnDetaching() {
			if( Messenger != null ) _listener?.Dispose();

			base.OnDetaching();
		}

		#region Dispose
		public void Dispose() {
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		private bool _disposed;

		protected virtual void Dispose( bool disposing ) {
			if( _disposed ) return;

			_listener?.Dispose();
			_disposed = true;
		}
		#endregion
	}
}