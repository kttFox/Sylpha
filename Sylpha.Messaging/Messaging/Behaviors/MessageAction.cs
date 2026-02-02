using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Xaml.Behaviors;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// ViewModelからのメッセージに対応するアクションの基底抽象クラスです<br />
	/// 独自のアクションを定義する場合はこのクラスを継承してください。
	/// </summary>
	/// <typeparam name="T">このアクションがアタッチ可能な型を示します。</typeparam>
	[ContentProperty( nameof( DirectMessage ) )]
	public abstract class MessageAction<T, TMessage> : TriggerAction<T> where T : DependencyObject where TMessage : Message {
		protected sealed override void Invoke( object? parameter ) {
			if( (bool)( DesignerProperties.IsInDesignModeProperty.GetMetadata( typeof( DependencyObject ) ).DefaultValue ) ) return;

			var window = Window.GetWindow( this.AssociatedObject );
			if( window == null ) {
				return;
			}

			var value = DirectMessage?.Message ?? parameter;
			if( value is TMessage message ) {
				InvokeAction( message );
				DirectMessage?.InvokeCallbacks( message );
			} else {
				Debug.WriteLine( $"パラメータとなるメッセージの型が {typeof(TMessage)} ではありません。Type: {value?.GetType()}" );
			}
		}

		protected abstract void InvokeAction( TMessage message );

		/// <summary>
		/// Viewで直接メッセージを定義する場合に使用する <see cref="Messaging.DirectMessage"/> を指定、または取得します。
		/// </summary>
		public DirectMessage? DirectMessage {
			get { return (DirectMessage?)GetValue( DirectMessageProperty ); }
			set { SetValue( DirectMessageProperty, value ); }
		}

		public static readonly DependencyProperty DirectMessageProperty =
			DependencyProperty.Register( nameof( DirectMessage ), typeof( DirectMessage ), typeof( MessageAction<T, TMessage> ), new PropertyMetadata( null ) );

	}
}