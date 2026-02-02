using System.ComponentModel;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using System.Windows.Markup;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// ViewModelからのメッセージに対応するアクションの基底抽象クラスです<br />
	/// 独自のアクションを定義する場合はこのクラスを継承してください。
	/// </summary>
	/// <typeparam name="T">このアクションがアタッチ可能な型を示します。</typeparam>
	[ContentProperty( nameof( DirectMessage ) )]
	public abstract class MessageAction<T> : TriggerAction<T> where T : DependencyObject {

		#region Register DirectMessageProperty
		/// <summary>
		/// Viewで直接メッセージを定義する場合に使用する、DirectMessageを指定、または取得します。
		/// </summary>
		public DirectMessage? DirectMessage {
			get { return (DirectMessage?)GetValue( DirectMessageProperty ); }
			set { SetValue( DirectMessageProperty, value ); }
		}

		public static readonly DependencyProperty DirectMessageProperty =
					DependencyProperty.Register( nameof( DirectMessage ), typeof( DirectMessage ), typeof( MessageAction<T> ), new PropertyMetadata( null ) );
		#endregion
		
		#region Register InvokeActionOnlyWhenWindowIsActive
		/// <summary>
		/// Windowがアクティブな時のみアクションを実行するかどうかを指定、または取得します。初期値はtrueです。
		/// </summary>
		public bool InvokeActionOnlyWhenWindowIsActive {
			get { return (bool)( GetValue( InvokeActionOnlyWhenWindowIsActiveProperty ) ?? default( bool ) ); }
			set { SetValue( InvokeActionOnlyWhenWindowIsActiveProperty, value ); }
		}

		public static readonly DependencyProperty InvokeActionOnlyWhenWindowIsActiveProperty =
			DependencyProperty.Register( nameof(InvokeActionOnlyWhenWindowIsActive), typeof( bool ), typeof( MessageAction<T> ), new PropertyMetadata( true ) );
		#endregion

		protected sealed override void Invoke( object parameter ) {
			var metadata = DesignerProperties.IsInDesignModeProperty.GetMetadata( typeof( DependencyObject ) );
			if( (bool)( metadata?.DefaultValue ?? false ) ) return;

			var message = parameter as Message;

			if( DirectMessage != null ) message = DirectMessage.Message;

			if( AssociatedObject == null ) return;

			var window = Window.GetWindow( AssociatedObject );
			if( window == null ) return;
			if( !window.IsActive && InvokeActionOnlyWhenWindowIsActive ) return;
			if( message == null ) return;

			InvokeAction( message );
			DirectMessage?.InvokeCallbacks( message );
		}

		protected abstract void InvokeAction( Message message );
	}
}