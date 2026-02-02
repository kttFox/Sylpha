using System;
using System.Windows;
using JetBrains.Annotations;

namespace Sylpha.Messaging {

	/// <summary>
	/// メソッドの名称にアクセスできるオブジェクトを表します。
	/// </summary>
	public interface ICallMethodMessage {
		string? MethodName { get; }
	}

	/// <summary>
	/// メソッドの名称と引数にアクセスできるオブジェクトを表します。
	/// </summary>
	public interface ICallOneParameterMethodMessage : ICallMethodMessage {
		object? MethodParameter { get; }
	}

	/// <summary>
	/// メソッドの返り値にアクセスできるオブジェクトを表します。
	/// </summary>
	public interface ICallFuncMessage : ICallMethodMessage {
		object? Result { get; set; }
	}

	/// <summary>
	///	メソッドを呼び出すメッセージの基底抽象クラスです。
	/// </summary>
	public abstract class CallMethodMessage : Message, ICallMethodMessage {
		public abstract string? MethodName { get; set; }
	}

	/// <summary>
	/// 引数の無いメソッドを呼び出すメッセージです。
	/// </summary>
	[PublicAPI]
	public class CallActionMessage : CallMethodMessage {
		public CallActionMessage() { }

		public CallActionMessage( string methodName ) {
			this.MethodName = methodName;
		}

		#region Register MethodName
		public sealed override string? MethodName {
			get => (string?)GetValue( MethodNameProperty );
			set => SetValue( MethodNameProperty, value );
		}

		public static readonly DependencyProperty MethodNameProperty =
			DependencyProperty.Register( nameof( MethodName ), typeof( string ), typeof( CallActionMessage ), new PropertyMetadata( default( string ) ) );
		#endregion

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		protected override Freezable CreateInstanceCore() => new CallActionMessage();
	}

	/// <summary>
	/// 引数を持つメソッドを呼び出すメッセージです。
	/// </summary>
	/// <typeparam name="TParameter">引数の型</typeparam>
	[PublicAPI]
	public class CallActionMessage<TParameter> : CallActionMessage, ICallOneParameterMethodMessage {

		public CallActionMessage() { }

		public CallActionMessage( string methodName, TParameter methodParameter ) : base( methodName ) {
			this.MethodParameter = methodParameter;
		}

		#region Register MethodParameter
		public TParameter MethodParameter {
			get => (TParameter)GetValue( MethodParameterProperty );
			set => SetValue( MethodParameterProperty, value );
		}

		public static readonly DependencyProperty MethodParameterProperty =
			DependencyProperty.Register( nameof( MethodParameter ), typeof( object ), typeof( CallActionMessage<TParameter> ), new PropertyMetadata( default( object ) ) );
		#endregion

		object? ICallOneParameterMethodMessage.MethodParameter => MethodParameter;


		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		protected override Freezable CreateInstanceCore() => new CallActionMessage<TParameter>();
	}


	/// <summary>
	/// 引数がなく、返り値のあるメソッドを呼び出すメッセージです。
	/// </summary>
	/// <typeparam name="TResult">返り値の型</typeparam>
	[PublicAPI]
	public class CallFuncMessage<TResult> : CallMethodMessage, ICallFuncMessage, IRequestMessage {
		public CallFuncMessage() { }

		public CallFuncMessage( string methodName ) {
			this.MethodName = methodName;
		}

		#region Register MethodName
		public sealed override string? MethodName {
			get => (string?)GetValue( MethodNameProperty );
			set => SetValue( MethodNameProperty, value );
		}

		public static readonly DependencyProperty MethodNameProperty =
			DependencyProperty.Register( nameof( MethodName ), typeof( string ), typeof( CallFuncMessage<TResult> ), new PropertyMetadata( default( string ) ) );
		#endregion


		public TResult? Result { get; set; }

		object? ICallFuncMessage.Result {
			get => Result;
			set => Result = (TResult?)value;
		}

		object? IRequestMessage.Response {
			get => Result;
			set => Result = (TResult?)value;
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		protected override Freezable CreateInstanceCore() => new CallFuncMessage<TResult>();
	}

	/// <summary>
	/// 引数と返り値のあるメソッドを呼び出すメッセージです。
	/// </summary>
	/// <typeparam name="TParameter">引数の型</typeparam>
	/// <typeparam name="TResult">返り値の型</typeparam>
	[PublicAPI]
	public class CallFuncMessage<TParameter, TResult> : CallFuncMessage<TResult>, ICallOneParameterMethodMessage {
		public CallFuncMessage() { }

		public CallFuncMessage( TParameter methodParameter ) {
			this.MethodParameter = methodParameter;
		}

		public CallFuncMessage( string methodName, TParameter methodParameter ) : base( methodName ) {
			this.MethodParameter = methodParameter;
		}

		#region Register MethodParameter
		public TParameter? MethodParameter {
			get => (TParameter?)GetValue( MethodParameterProperty );
			set => SetValue( MethodParameterProperty, value );
		}

		public static readonly DependencyProperty MethodParameterProperty =
			DependencyProperty.Register( nameof( MethodParameter ), typeof( TParameter ), typeof( CallFuncMessage<TParameter?, TResult?> ), new PropertyMetadata( default( TParameter ) ) );
		#endregion

		object? ICallOneParameterMethodMessage.MethodParameter => MethodParameter;

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		protected override Freezable CreateInstanceCore() => new CallFuncMessage<TParameter, TResult>();
	}

	public static class CallMethodMessageExtensions {
		/// <summary>
		/// 引数の無いメソッドを呼び出すメッセージを送信します。
		/// </summary>
		/// <param name="messenger">送信先</param>
		/// <param name="message">送信するメッセージ</param>
		public static void CallAction( this Messenger messenger, CallActionMessage message ) {
			messenger.Raise( message );
		}

		/// <summary>
		/// 引数を持つメソッドを呼び出すメッセージを送信します。
		/// </summary>
		/// <typeparam name="TParameter">メソッドの引数の型</typeparam>
		/// <param name="messenger">送信先</param>
		/// <param name="message">送信するメッセージ</param>
		public static void CallAction<TParameter>( this Messenger messenger, CallActionMessage<TParameter> message ) {
			messenger.Raise( message );
		}

		/// <summary>
		/// 引数が無く、返り値を持つメソッドを呼び出すメッセージを送信し、結果を取得します。
		/// </summary>
		/// <typeparam name="TResult">メソッドの返り値の型</typeparam>
		/// <param name="messenger">送信先</param>
		/// <param name="message">送信するメッセージ</param>
		/// <returns>結果が設定されたメッセージ</returns>
		public static CallFuncMessage<TResult> CallFunc<TResult>( this Messenger messenger, CallFuncMessage<TResult> message ) {
			return messenger.Raise( message );
		}

		/// <summary>
		/// 引数と返り値を持つメソッドを呼び出すメッセージを送信し、結果を取得します。
		/// </summary>
		/// <typeparam name="TParameter">メソッドの引数の型</typeparam>
		/// <typeparam name="TResult">メソッドの返り値の型</typeparam>
		/// <param name="messenger">送信先</param>
		/// <param name="message">送信するメッセージ</param>
		/// <returns>結果が設定されたメッセージ</returns>
		public static CallFuncMessage<TParameter, TResult> CallFunc<TParameter, TResult>( this Messenger messenger, CallFuncMessage<TParameter, TResult> message ) {
			return messenger.Raise( message );
		}
	}
}
