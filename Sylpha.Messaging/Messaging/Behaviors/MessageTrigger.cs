using System;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using Sylpha.EventListeners.WeakEvents;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// ViewModelからのメッセージを受信し、アクションを実行するトリガー
	/// </summary>
	public class MessageTrigger : TriggerBase<FrameworkElement>, IDisposable {
		#region Register MessengerProperty
		/// <summary>
		/// ViewModelのMessengerを指定、または取得します。
		/// </summary>
		public Messenger? Messenger {
			get => (Messenger?)GetValue( MessengerProperty );
			set => SetValue( MessengerProperty, value );
		}

		/// <summary>
		/// <see cref="Messenger"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty MessengerProperty =
					DependencyProperty.Register( nameof( Messenger ), typeof( Messenger ), typeof( MessageTrigger ), new PropertyMetadata( null, MessengerChanged ) );

		private static void MessengerChanged( DependencyObject d, DependencyPropertyChangedEventArgs e ) {
			var sender = (MessageTrigger)d;

			if( e.OldValue == e.NewValue ) return;

			if( e.OldValue != null ) {
				sender._listener?.Dispose();
			}

			if( e.NewValue is Messenger messenger ) {
				sender._listener =
					new WeakEventListener<EventHandler<MessageRaisedEventArgs>, MessageRaisedEventArgs>(
						h => h,
						h => messenger.Raised += h,
						h => messenger.Raised -= h,
						sender.MessageReceived );
			}
		}

		private void MessageReceived( object? sender, MessageRaisedEventArgs e ) {
			var message = e.Message;

			var cloneMessage = (Message)message.Clone();
			cloneMessage.Freeze();

			if( MessageKey != cloneMessage.MessageKey ) {
				return;
			}

			InvokeActions( cloneMessage );

			if( message is IRequestMessage requestMessage ) {
				requestMessage.Response = ( (IRequestMessage)cloneMessage ).Response;
			}
			if( message is ShowWindowMessage showWindowMessage ) {
				showWindowMessage.WindowViewModel = ( (ShowWindowMessage)cloneMessage ).WindowViewModel;
			}
		}
		#endregion

		private WeakEventListener<EventHandler<MessageRaisedEventArgs>, MessageRaisedEventArgs>? _listener;

		/// <summary>
		/// このトリガーが反応するメッセージのメッセージキーを指定、または取得します。<br />
		/// このプロパティが指定されていない場合、このトリガーは全てのメッセージに反応します。
		/// </summary>
		public string? MessageKey { get; set; }

		/// <inheritdoc />
		protected override void OnAttached() {
			base.OnAttached();
		}

		/// <inheritdoc />
		protected override void OnDetaching() {
			if( Messenger != null ) Dispose();

			base.OnDetaching();
		}

		#region Dispose
		private bool _disposed;

		/// <summary>
		/// リスナーを破棄します。
		/// </summary>
		public virtual void Dispose() {
			if( !_disposed ) {
				_disposed = true;

				_listener?.Dispose();
			}
		}
		#endregion
	}
}