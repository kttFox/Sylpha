using System.Windows;

namespace Sylpha.Messaging {

	/// <summary>
	/// メソッドの名称にアクセスできるオブジェクトを表します。
	/// </summary>
	public interface ICallMethodMessage {
		/// <summary>
		/// 呼び出すメソッドの名称を取得します。
		/// </summary>
		string? MethodName { get; }
	}

	/// <summary>
	/// メソッドの名称と引数にアクセスできるオブジェクトを表します。
	/// </summary>
	public interface ICallOneParameterMethodMessage : ICallMethodMessage {
		/// <summary>
		/// 呼び出すメソッドの引数を取得します。
		/// </summary>
		object? MethodParameter { get; }
	}

	/// <summary>
	/// メソッドの戻り値にアクセスできるオブジェクトを表します。
	/// </summary>
	public interface ICallFuncMessage : ICallMethodMessage {
		/// <summary>
		/// 呼び出したメソッドの戻り値を取得または設定します。
		/// </summary>
		object? Result { get; set; }
	}

	/// <summary>
	/// メソッドを呼び出すメッセージの基底抽象クラス
	/// </summary>
	public abstract class CallMethodMessage : Message, ICallMethodMessage {
		/// <summary>
		/// 呼び出すメソッドの名称を取得または設定します。
		/// </summary>
		public abstract string? MethodName { get; set; }
	}

	/// <summary>
	/// 引数の無いメソッドを呼び出すメッセージ
	/// </summary>
	public class CallActionMessage : CallMethodMessage {
		/// <summary>
		/// <see cref="CallActionMessage"/> の新しいインスタンスを初期化します。
		/// </summary>
		public CallActionMessage() { }

		/// <summary>
		/// 呼び出すメソッドの名称を指定して、<see cref="CallActionMessage"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="methodName">呼び出すメソッドの名称</param>
		public CallActionMessage( string methodName ) {
			this.MethodName = methodName;
		}

		#region Register MethodName
		/// <inheritdoc />
		public sealed override string? MethodName {
			get => (string?)GetValue( MethodNameProperty );
			set => SetValue( MethodNameProperty, value );
		}

		/// <summary>
		/// <see cref="MethodName"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty MethodNameProperty =
			DependencyProperty.Register( nameof( MethodName ), typeof( string ), typeof( CallActionMessage ), new PropertyMetadata( default( string ) ) );
		#endregion

		/// <inheritdoc />
		protected override Freezable CreateInstanceCore() => new CallActionMessage();
	}

	/// <summary>
	/// 引数を持つメソッドを呼び出すメッセージ
	/// </summary>
	/// <typeparam name="TParameter">メソッドの引数の型</typeparam>
	public class CallActionMessage<TParameter> : CallActionMessage, ICallOneParameterMethodMessage {
		/// <summary>
		/// <see cref="CallActionMessage{TParameter}"/> の新しいインスタンスを初期化します。
		/// </summary>
		public CallActionMessage() { }

		/// <summary>
		/// 呼び出すメソッドの名称と引数を指定して、<see cref="CallActionMessage{TParameter}"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="methodName">呼び出すメソッドの名称</param>
		/// <param name="methodParameter">呼び出すメソッドの引数</param>
		public CallActionMessage( string methodName, TParameter methodParameter ) : base( methodName ) {
			this.MethodParameter = methodParameter;
		}

		#region Register MethodParameter
		/// <summary>
		/// 呼び出すメソッドの引数を取得または設定します。
		/// </summary>
		public TParameter MethodParameter {
			get => (TParameter)GetValue( MethodParameterProperty );
			set => SetValue( MethodParameterProperty, value );
		}

		/// <summary>
		/// <see cref="MethodParameter"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty MethodParameterProperty =
			DependencyProperty.Register( nameof( MethodParameter ), typeof( object ), typeof( CallActionMessage<TParameter> ), new PropertyMetadata( default( object ) ) );
		#endregion

		object? ICallOneParameterMethodMessage.MethodParameter => MethodParameter;


		/// <inheritdoc />
		protected override Freezable CreateInstanceCore() => new CallActionMessage<TParameter>();
	}


	/// <summary>
	/// 引数がなく、戻り値のあるメソッドを呼び出すメッセージ
	/// </summary>
	/// <typeparam name="TResult">メソッドの戻り値の型</typeparam>
	public class CallFuncMessage<TResult> : CallMethodMessage, ICallFuncMessage, IRequestMessage {
		/// <summary>
		/// <see cref="CallFuncMessage{TResult}"/> の新しいインスタンスを初期化します。
		/// </summary>
		public CallFuncMessage() { }

		/// <summary>
		/// 呼び出すメソッドの名称を指定して、<see cref="CallFuncMessage{TResult}"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="methodName">呼び出すメソッドの名称</param>
		public CallFuncMessage( string methodName ) {
			this.MethodName = methodName;
		}

		#region Register MethodName
		/// <inheritdoc />
		public sealed override string? MethodName {
			get => (string?)GetValue( MethodNameProperty );
			set => SetValue( MethodNameProperty, value );
		}

		/// <summary>
		/// <see cref="MethodName"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty MethodNameProperty =
			DependencyProperty.Register( nameof( MethodName ), typeof( string ), typeof( CallFuncMessage<TResult> ), new PropertyMetadata( default( string ) ) );
		#endregion

		/// <summary>
		/// 呼び出したメソッドの戻り値を取得または設定します。
		/// </summary>
		public TResult? Result { get; set; }

		/// <inheritdoc />
		object? ICallFuncMessage.Result {
			get => Result;
			set => Result = (TResult?)value;
		}

		/// <inheritdoc />
		object? IRequestMessage.Response {
			get => Result;
			set => Result = (TResult?)value;
		}

		/// <inheritdoc />
		protected override Freezable CreateInstanceCore() => new CallFuncMessage<TResult>();
	}

	/// <summary>
	/// 引数と戻り値のあるメソッドを呼び出すメッセージ
	/// </summary>
	/// <typeparam name="TParameter">メソッドの引数の型</typeparam>
	/// <typeparam name="TResult">メソッドの戻り値の型</typeparam>
	public class CallFuncMessage<TParameter, TResult> : CallFuncMessage<TResult>, ICallOneParameterMethodMessage {
		/// <summary>
		/// <see cref="CallFuncMessage{TParameter,TResult}"/> の新しいインスタンスを初期化します。
		/// </summary>
		public CallFuncMessage() { }

		/// <summary>
		/// 呼び出すメソッドの名称と引数を指定して、<see cref="CallFuncMessage{TParameter,TResult}"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="methodName">呼び出すメソッドの名称</param>
		/// <param name="methodParameter">呼び出すメソッドの引数</param>
		public CallFuncMessage( string methodName, TParameter methodParameter ) : base( methodName ) {
			this.MethodParameter = methodParameter;
		}

		#region Register MethodParameter
		/// <summary>
		/// 呼び出すメソッドの引数を取得または設定します。
		/// </summary>
		public TParameter? MethodParameter {
			get => (TParameter?)GetValue( MethodParameterProperty );
			set => SetValue( MethodParameterProperty, value );
		}

		/// <summary>
		/// <see cref="MethodParameter"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty MethodParameterProperty =
			DependencyProperty.Register( nameof( MethodParameter ), typeof( TParameter ), typeof( CallFuncMessage<TParameter?, TResult?> ), new PropertyMetadata( default( TParameter ) ) );
		#endregion

		object? ICallOneParameterMethodMessage.MethodParameter => MethodParameter;

		/// <inheritdoc />
		protected override Freezable CreateInstanceCore() => new CallFuncMessage<TParameter, TResult>();
	}

	/// <summary>
	/// <see cref="CallMethodMessage"/> 用の拡張メソッドを提供します。
	/// </summary>
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
		/// 引数が無く、戻り値を持つメソッドを呼び出すメッセージを送信し、結果を取得します。
		/// </summary>
		/// <typeparam name="TResult">メソッドの戻り値の型</typeparam>
		/// <param name="messenger">送信先</param>
		/// <param name="message">送信するメッセージ</param>
		/// <returns>結果が設定されたメッセージ</returns>
		public static CallFuncMessage<TResult> CallFunc<TResult>( this Messenger messenger, CallFuncMessage<TResult> message ) {
			return messenger.Raise( message );
		}

		/// <summary>
		/// 引数と戻り値を持つメソッドを呼び出すメッセージを送信し、結果を取得します。
		/// </summary>
		/// <typeparam name="TParameter">メソッドの引数の型</typeparam>
		/// <typeparam name="TResult">メソッドの戻り値の型</typeparam>
		/// <param name="messenger">送信先</param>
		/// <param name="message">送信するメッセージ</param>
		/// <returns>結果が設定されたメッセージ</returns>
		public static CallFuncMessage<TParameter, TResult> CallFunc<TParameter, TResult>( this Messenger messenger, CallFuncMessage<TParameter, TResult> message ) {
			return messenger.Raise( message );
		}
	}
}
