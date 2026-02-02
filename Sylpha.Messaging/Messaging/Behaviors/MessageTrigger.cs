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
				new WeakEventListener<EventHandler<MessageRaisedEventArgs>,
					MessageRaisedEventArgs>(
					h => h,
					h => newMessenger.Raised += h,
					h => newMessenger.Raised -= h,
					thisReference.MessageReceived );
		}
		#endregion

		#region Register InvokeActionsOnlyWhileAttachedObjectLoaded
		/// <summary>
		/// アタッチされたオブジェクトがロードされている期間(Loaded~Unloaded)だけActionを実行するかどうかを指定、または取得します。デフォルトはfalseです。
		/// </summary>
		public bool InvokeActionsOnlyWhileAttachedObjectLoaded {
			get { return (bool)( GetValue( InvokeActionsOnlyWhileAttachedObjectLoadedProperty ) ?? default( bool ) ); }
			set { SetValue( InvokeActionsOnlyWhileAttachedObjectLoadedProperty, value ); }
		}
		public static readonly DependencyProperty InvokeActionsOnlyWhileAttachedObjectLoadedProperty =
			DependencyProperty.Register( nameof( InvokeActionsOnlyWhileAttachedObjectLoaded ), typeof( bool ), typeof( MessageTrigger ), new PropertyMetadata( false ) );
		#endregion

		#region Register IsEnable
		/// <summary>
		/// このトリガーが有効かどうかを指定、または取得します。デフォルトはtrueです。
		/// </summary>
		public bool IsEnable {
			get { return (bool)( GetValue( IsEnableProperty ) ); }
			set { SetValue( IsEnableProperty, value ); }
		}
		public static readonly DependencyProperty IsEnableProperty =
			DependencyProperty.Register( nameof( IsEnable ), typeof( Messenger ), typeof( MessageTrigger ), new PropertyMetadata( true ) );
		#endregion

		private WeakEventListener<EventHandler<MessageRaisedEventArgs>, MessageRaisedEventArgs>? _listener;

		private bool _loaded = true;

		/// <summary>
		/// このトリガーが反応するメッセージのメッセージキーを指定、または取得します。<br />
		/// このプロパティが指定されていない場合、このトリガーは全てのメッセージに反応します。
		/// </summary>
		public string? MessageKey { get; set; }

		public void Dispose() {
			Dispose( true );
			GC.SuppressFinalize( this );
		}
		private void MessageReceived( object? sender, MessageRaisedEventArgs e ) {
			if( e == null ) throw new ArgumentNullException( nameof( e ) );

			var message = e.Message;

			var cloneMessage = (Message)message.Clone();

			cloneMessage.Freeze();

			var checkResult = false;

			void CheckAction() {
				if( !IsEnable ) return;
				if( this.InvokeActionsOnlyWhileAttachedObjectLoaded && !_loaded ) return;
				if( !( string.IsNullOrEmpty( MessageKey ) || MessageKey == cloneMessage.MessageKey ) ) return;

				checkResult = true;
			}

			DoActionOnDispatcher( CheckAction );

			if( !checkResult ) return;

			DoActionOnDispatcher( () => InvokeActions( cloneMessage ) );


			if( message is IRequest responsiveMessage ) {
				object? response;
				if( ( response = ( (IRequest)cloneMessage ).Response ) != null )
					responsiveMessage.Response = response;
			}
		}

		private void DoActionOnDispatcher( Action action ) {
			if( action == null ) throw new ArgumentNullException( nameof( action ) );
			if( Dispatcher == null ) throw new InvalidOperationException( "Dispatcher is null." );

			if( Dispatcher.CheckAccess() )
				action();
			else
				Dispatcher.Invoke( action );
		}

		protected override void OnAttached() {
			base.OnAttached();

			if( AssociatedObject == null ) return;

			AssociatedObject.Loaded += AssociatedObjectLoaded;
			AssociatedObject.Unloaded += AssociatedObjectUnloaded;
		}

		private void AssociatedObjectLoaded( object sender, RoutedEventArgs e ) {
			_loaded = true;
		}

		private void AssociatedObjectUnloaded( object sender, RoutedEventArgs e ) {
			_loaded = false;
		}

		protected override void OnDetaching() {
			if( Messenger != null ) _listener?.Dispose();

			if( AssociatedObject != null ) {
				AssociatedObject.Loaded -= AssociatedObjectLoaded;
				AssociatedObject.Unloaded -= AssociatedObjectUnloaded;
			}

			base.OnDetaching();
		}

		private bool _disposed;

		protected virtual void Dispose( bool disposing ) {
			if( _disposed ) return;

			_listener?.Dispose();
			_disposed = true;
		}
	}
}