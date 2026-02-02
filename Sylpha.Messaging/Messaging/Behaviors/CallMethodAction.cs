using System;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// 引数を一つだけ持つメソッドに対応したアクションです。
	/// </summary>
	public class CallMethodAction : TriggerAction<DependencyObject> {
		#region Register MethodTarget
		/// <summary>
		/// メソッドを呼び出すオブジェクトを指定、または取得します。
		/// </summary>
		public object? MethodTarget {
			get => (object?)GetValue( MethodTargetProperty );
			set => SetValue( MethodTargetProperty, value );
		}

		public static readonly DependencyProperty MethodTargetProperty =
			DependencyProperty.Register( nameof( MethodTarget ), typeof( object ), typeof( CallMethodAction ), new PropertyMetadata( default( object? ) ) );
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
			DependencyProperty.Register( nameof( MethodName ), typeof( string ), typeof( CallMethodAction ), new PropertyMetadata( default( string? ) ) );
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
			DependencyProperty.Register( nameof( MethodParameter ), typeof( object ), typeof( CallMethodAction ), new PropertyMetadata( default( object? ), OnMethodParameterChanged ) );
		
		private static void OnMethodParameterChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e ) {
			var action = sender as CallMethodAction ?? throw new ArgumentException( $"Value must be a {nameof( CallMethodAction )}.", nameof( sender ) );

			action._parameterSet = true;
		}
		#endregion

		private readonly MethodBinderWithArgument _callbackMethod = new();
		private readonly MethodBinder _method = new();

		private bool _parameterSet;


		protected override void Invoke( object parameter ) {
			if( MethodTarget == null ) return;
			if( MethodName == null ) return;

			if( !_parameterSet )
				_method.Invoke( MethodTarget, MethodName );
			else
				_callbackMethod.Invoke( MethodTarget, MethodName, MethodParameter );
		}
	}
}