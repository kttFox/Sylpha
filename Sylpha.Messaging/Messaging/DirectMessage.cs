using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace Sylpha.Messaging {
	/// <summary>
	/// Viewから直接メッセージを定義する際に使用します。
	/// </summary>
	[ContentProperty( nameof(Message) )]
	public class DirectMessage : Freezable {
		// Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register( nameof( Message ), typeof( Message ), typeof( DirectMessage ), new UIPropertyMetadata( null ) );

		// Using a DependencyProperty as the backing store for CallbackCommand.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CallbackCommandProperty =
			DependencyProperty.Register( nameof( CallbackCommand ), typeof( ICommand ), typeof( DirectMessage ), new UIPropertyMetadata( null ) );

		// Using a DependencyProperty as the backing store for CallbackMethodTarget.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CallbackMethodTargetProperty =
			DependencyProperty.Register( nameof( CallbackMethodTarget ), typeof( object ), typeof( DirectMessage ), new UIPropertyMetadata( null ) );

		// Using a DependencyProperty as the backing store for MethodName.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CallbackMethodNameProperty =
			DependencyProperty.Register( nameof( CallbackMethodName ), typeof( string ), typeof( DirectMessage ), new UIPropertyMetadata( null ) );

		private readonly MethodBinderWithArgument _callbackMethod = new();

		/// <summary>
		/// メッセージ(各種Message)を指定、または取得します。
		/// </summary>
		public Message Message {
			get { return (Message)GetValue( MessageProperty ); }
			set { SetValue( MessageProperty, value ); }
		}

		/// <summary>
		/// アクション実行後に実行するコマンドを指定、または取得します<br />
		/// このプロパティが設定されていた場合、アクションの実行後アクションの実行に使用したメッセージをパラメータとしてコマンドを呼び出します。
		/// </summary>
		public ICommand CallbackCommand {
			get { return (ICommand)GetValue( CallbackCommandProperty ); }
			set { SetValue( CallbackCommandProperty, value ); }
		}

		/// <summary>
		/// アクション実行後に実行するメソッドを持つオブジェクトを指定、または取得します<br />
		/// このプロパティとCallbackMethodNameが設定されていた場合、アクションの実行後アクションの実行に使用したメッセージをパラメータとしてメソッドを呼び出します。
		/// </summary>
		public object CallbackMethodTarget {
			get { return GetValue( CallbackMethodTargetProperty ); }
			set { SetValue( CallbackMethodTargetProperty, value ); }
		}

		/// <summary>
		/// アクション実行後に実行するメソッドの名前を指定、または取得します<br />
		/// このプロパティとCallbackMethodTargetが設定されていた場合、アクションの実行後アクションの実行に使用したメッセージをパラメータとしてメソッドを呼び出します。
		/// </summary>
		public string CallbackMethodName {
			get { return (string)GetValue( CallbackMethodNameProperty ); }
			set { SetValue( CallbackMethodNameProperty, value ); }
		}

		internal void InvokeCallbacks( Message message ) {
			if( message == null ) throw new ArgumentNullException( nameof( message ) );

			if( CallbackCommand != null ) {
				if( CallbackCommand.CanExecute( message ) )
					CallbackCommand.Execute( message );
			}

			if( CallbackMethodTarget != null && CallbackMethodName != null )
				_callbackMethod.Invoke( CallbackMethodTarget, CallbackMethodName, message );
		}

		protected override Freezable CreateInstanceCore() {
			return new DirectMessage();
		}
	}
}