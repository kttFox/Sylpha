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
	/// <typeparam name="TMessage">このアクションがアタッチ可能な型を示します。</typeparam>
	[ContentProperty( nameof( DirectMessage ) )]
	public abstract class MessageAction<T, TMessage> : TriggerAction<T> where T : DependencyObject where TMessage : Message {
		#region Register DirectMessage
		/// <summary>
		/// Viewで直接メッセージを定義する場合に使用する <see cref="Messaging.DirectMessage"/> を指定、または取得します。
		/// </summary>
		public DirectMessage? DirectMessage {
			get => (DirectMessage?)GetValue( DirectMessageProperty );
			set => SetValue( DirectMessageProperty, value );
		}

		/// <summary>
		/// <see cref="DirectMessage"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty DirectMessageProperty =
			DependencyProperty.Register( nameof( DirectMessage ), typeof( DirectMessage ), typeof( MessageAction<T, TMessage> ), new PropertyMetadata( null ) );
		#endregion

		/// <inheritdoc />
		protected sealed override void Invoke( object? parameter ) {
			if( (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata( typeof( DependencyObject ) ).DefaultValue ) return;

			var window = Window.GetWindow( this.AssociatedObject );
			if( window == null ) {
				return;
			}

			var param = DirectMessage?.Message ?? parameter;
			if( param is TMessage message ) {
				InvokeAction( message );
				DirectMessage?.InvokeCallbacks( message );
			} else {
				InvokeAction( param );
			}
		}

		/// <summary>
		/// パラメーターの型が <typeparamref name="TMessage"/> の場合に呼び出します。
		/// </summary>
		/// <param name="message">処理するメッセージオブジェクト</param>
		protected abstract void InvokeAction( TMessage message );

		/// <summary>
		/// パラメーターの型が <typeparamref name="TMessage"/> 以外の場合に呼び出します。
		/// </summary>
		/// <param name="parameter">型が <typeparamref name="TMessage"/> ではないパラメーター</param>
		protected virtual void InvokeAction( object? parameter ) {
			Debug.WriteLine( $"パラメーターとなるメッセージの型が {typeof( TMessage )} ではありません。Type: {parameter?.GetType()}" );
		}
	}
}