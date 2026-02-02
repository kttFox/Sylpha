using System.ComponentModel;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using System.Windows.Markup;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// ViewModelからの相互作用メッセージに対応するアクションの基底抽象クラスです<br />
	/// 独自のアクションを定義する場合はこのクラスを継承してください。
	/// </summary>
	/// <typeparam name="T">このアクションがアタッチ可能な型を示します。</typeparam>
	[ContentProperty( nameof(DirectMessage) )]
	public abstract class MessageAction<T> : TriggerAction<T> where T : DependencyObject {
		// Using a DependencyProperty as the backing store for DirectMessage.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DirectMessageProperty =
			DependencyProperty.Register( nameof(DirectMessage), typeof( DirectMessage ), typeof( MessageAction<T> ), new PropertyMetadata() );

		// Using a DependencyProperty as the backing store for InvokeActionOnlyWhenWindowIsActive.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty InvokeActionOnlyWhenWindowIsActiveProperty =
			DependencyProperty.Register( nameof(InvokeActionOnlyWhenWindowIsActive), typeof( bool ), typeof( MessageAction<T> ), new PropertyMetadata( true ) );

		/// <summary>
		/// Viewで直接メッセージを定義する場合に使用する、DirectMessageを指定、または取得します。
		/// </summary>
		public DirectMessage DirectMessage {
			get { return (DirectMessage)GetValue( DirectMessageProperty ); }
			set { SetValue( DirectMessageProperty, value ); }
		}

		/// <summary>
		/// Windowがアクティブな時のみアクションを実行するかどうかを指定、または取得します。初期値はtrueです。
		/// </summary>
		public bool InvokeActionOnlyWhenWindowIsActive {
			get { return (bool)( GetValue( InvokeActionOnlyWhenWindowIsActiveProperty ) ?? default( bool ) ); }
			set { SetValue( InvokeActionOnlyWhenWindowIsActiveProperty, value ); }
		}

		protected sealed override void Invoke( object parameter ) {
			var metadata = DesignerProperties.IsInDesignModeProperty.GetMetadata( typeof( DependencyObject ) );
			if( (bool)( metadata?.DefaultValue ?? false ) ) return;

			var message = parameter as Message;

			if( this.DirectMessage != null ) message = this.DirectMessage.Message;

			if( AssociatedObject == null ) return;

			var window = Window.GetWindow( AssociatedObject );
			if( window == null ) return;
			if( !window.IsActive && InvokeActionOnlyWhenWindowIsActive ) return;
			if( message == null ) return;

			InvokeAction( message );
			this.DirectMessage?.InvokeCallbacks( message );
		}

		protected abstract void InvokeAction( Message message );
	}
}