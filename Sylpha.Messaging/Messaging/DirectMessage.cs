using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace Sylpha.Messaging {
	/// <summary>
	/// View から直接メッセージを定義する際に使用します。
	/// </summary>
	[ContentProperty( nameof( Message ) )]
	public class DirectMessage : Freezable {
		#region Register MessageProperty
		/// <summary>
		/// メッセージを指定、または取得します。
		/// </summary>
		public Message? Message {
			get => (Message?)GetValue( MessageProperty );
			set => SetValue( MessageProperty, value );
		}
		
		/// <summary>
		/// <see cref="Message"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty MessageProperty =
					DependencyProperty.Register( nameof( Message ), typeof( Message ), typeof( DirectMessage ), new UIPropertyMetadata( null ) );
		#endregion

		#region Register CallbackCommandProperty
		/// <summary>
		/// アクション実行後に実行するコマンドを指定、または取得します。<br />
		/// このプロパティが設定されている場合、アクションの実行後に、アクションの実行に使用したメッセージをパラメーターとしてコマンドを呼び出します。
		/// </summary>
		public ICommand? CallbackCommand {
			get => (ICommand?)GetValue( CallbackCommandProperty );
			set => SetValue( CallbackCommandProperty, value );
		}
		
		/// <summary>
		/// <see cref="CallbackCommand"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty CallbackCommandProperty =
					DependencyProperty.Register( nameof( CallbackCommand ), typeof( ICommand ), typeof( DirectMessage ), new UIPropertyMetadata( null ) );
		#endregion

		#region Register CallbackMethodTargetProperty
		/// <summary>
		/// アクション実行後に実行するメソッドを持つオブジェクトを指定、または取得します。<br />
		/// このプロパティと <see cref="CallbackMethodName"/> が設定されている場合、アクションの実行後に、アクションの実行に使用したメッセージをパラメーターとしてメソッドを呼び出します。
		/// </summary>
		public object? CallbackMethodTarget {
			get => (object?)GetValue( CallbackMethodTargetProperty );
			set => SetValue( CallbackMethodTargetProperty, value );
		}
		
		/// <summary>
		/// <see cref="CallbackMethodTarget"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty CallbackMethodTargetProperty =
					DependencyProperty.Register( nameof( CallbackMethodTarget ), typeof( object ), typeof( DirectMessage ), new UIPropertyMetadata( null ) );
		#endregion

		#region Register CallbackMethodNameProperty
		/// <summary>
		/// アクション実行後に実行するメソッドの名前を指定、または取得します。<br />
		/// このプロパティと <see cref="CallbackMethodTarget"/> が設定されている場合、アクションの実行後に、アクションの実行に使用したメッセージをパラメーターとしてメソッドを呼び出します。
		/// </summary>
		public string? CallbackMethodName {
			get => (string?)GetValue( CallbackMethodNameProperty );
			set => SetValue( CallbackMethodNameProperty, value );
		}

		/// <summary>
		/// <see cref="CallbackMethodName"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty CallbackMethodNameProperty =
					DependencyProperty.Register( nameof( CallbackMethodName ), typeof( string ), typeof( DirectMessage ), new UIPropertyMetadata( null ) );
		#endregion

		private readonly MethodBinderWithArgument _callbackMethod = new();

		internal void InvokeCallbacks( Message message ) {
			if( message == null ) throw new ArgumentNullException( nameof( message ) );

			if( CallbackCommand != null ) {
				if( CallbackCommand.CanExecute( message ) ) {
					CallbackCommand.Execute( message );
				}
			}

			if( CallbackMethodTarget != null && CallbackMethodName != null ) {
				_callbackMethod.Invoke( CallbackMethodTarget, CallbackMethodName, message.GetType(), message );
			}
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。<see cref="Freezable"/> オブジェクトとして必要な実装です。<br />
		/// 通常、このメソッドは自身の新しいインスタンスを返すように実装します。
		/// </summary>
		protected override Freezable CreateInstanceCore() => new DirectMessage();
	}
}