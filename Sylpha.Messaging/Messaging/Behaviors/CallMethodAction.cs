using System;
using System.Linq;
using System.Windows;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// 引数を一つだけ持つメソッドに対応したアクションです。
	/// </summary>
	public class CallMethodAction : MessageAction<FrameworkElement, CallMethodMessage> {
		#region Register TargetType
		/// <summary>
		/// メソッドを呼び出すオブジェクトを指定、または取得します。
		/// </summary>
		public object? MethodTarget {
			get => (object?)GetValue( MethodTargetProperty );
			set => SetValue( MethodTargetProperty, value );
		}

		public static readonly DependencyProperty MethodTargetProperty =
			DependencyProperty.Register( nameof( MethodTarget ), typeof( object ), typeof( CallMethodAction ), new PropertyMetadata( null ) );
		#endregion

		#region Register MethodName
		/// <summary>
		/// 呼び出すメソッドの名前を指定、または取得します。
		/// </summary>
		public string? MethodName {
			get => (string?)GetValue( MethodNameProperty );
			set => SetValue( MethodNameProperty, value );
		}

		public static readonly DependencyProperty MethodNameProperty =
			DependencyProperty.Register( nameof( MethodName ), typeof( string ), typeof( CallMethodAction ), new PropertyMetadata( null ) );
		#endregion

		#region Register MethodParameter
		/// <summary>
		/// 呼び出すメソッドに渡す引数を指定、または取得します。
		/// </summary>
		public object? MethodParameter {
			get => (object?)GetValue( MethodParameterProperty );
			set => SetValue( MethodParameterProperty, value );
		}

		public static readonly DependencyProperty MethodParameterProperty =
			DependencyProperty.Register( nameof( MethodParameter ), typeof( object ), typeof( CallMethodAction ), new PropertyMetadata( null ) );
		#endregion

		#region Register MethodParameterType
		public Type? MethodParameterType {
			get => (Type?)GetValue( MethodParameterTypeProperty );
			set => SetValue( MethodParameterTypeProperty, value );
		}

		public static readonly DependencyProperty MethodParameterTypeProperty =
			DependencyProperty.Register( nameof( MethodParameterType ), typeof( Type ), typeof( CallMethodAction ), new PropertyMetadata( null ) );
		#endregion

		private static readonly MethodBinder Method = new();
		private static readonly MethodBinderWithArgument ArgMethod = new();

		protected override void InvokeAction( object? parameter ) {
			Action( MethodTarget ?? this.AssociatedObject, MethodName, MethodParameterType, MethodParameter, null );
		}

		protected override void InvokeAction( CallMethodMessage message ) {
			Action( MethodTarget ?? this.AssociatedObject, MethodName, MethodParameterType, MethodParameter, message );
		}

		/// <summary>
		/// 指定されたターゲット上のメソッドを呼び出します。
		/// </summary>
		/// <param name="methodTarget">メソッドを呼び出すオブジェクト</param>
		/// <param name="methodName">呼び出すメソッドの名前</param>
		/// <param name="methodParameterType">メソッドに渡す引数の型</param>
		/// <param name="methodParameter">メソッドに渡す引数のインスタンス</param>
		/// <param name="message">呼び出しのための追加情報を含むメッセージ</param>
		public static void Action( object methodTarget, string? methodName, Type? methodParameterType, object? methodParameter, CallMethodMessage? message ) {
			if( message is ICallMethodMessage callMethodMessage ) {
				methodName = callMethodMessage.MethodName ?? methodName;
			}
			if( methodName == null ) throw new ArgumentNullException( nameof( methodName ) );

			if( message is ICallOneParameterMethodMessage callMethodMessageOneParameter ) {
				methodParameter = callMethodMessageOneParameter.MethodParameter ?? methodParameter;
			}

			var t = message?.GetType();
			if( t?.IsGenericType == true ) {
				var type = t.GetGenericTypeDefinition();
				if( type == typeof( CallActionMessage<> ) || type == typeof( CallFuncMessage<,> ) ) {
					methodParameterType = t.GenericTypeArguments.First();
				}
			}

			var result = ( methodParameterType == null && methodParameter == null ) switch {
				true => Method.Invoke( methodTarget, methodName ),
				false => ArgMethod.Invoke( methodTarget, methodName, methodParameterType ?? methodParameter!.GetType(), methodParameter )
			};
			if( message is ICallFuncMessage funcMessage ) {
				funcMessage.Result = result;
			}

		}

	}
}