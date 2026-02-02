using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Sylpha.Messaging {

	/// <summary>
	/// 単一引数を持つインスタンスメソッドをリフレクションで取得し、呼び出すクラスです。
	/// </summary>
	public class MethodBinderWithArgument {
		private static readonly ConcurrentDictionary<(Type targetType, string methodName, Type argumentType), Action<object, object?>> ActionCacheDictionary = [];
		private static readonly ConcurrentDictionary<(Type targetType, string methodName, Type argumentType), Func<object, object?, object>> FuncCacheDictionary = [];

		private static readonly List<Task> TaskList = [];

		/// <summary>
		/// 現在バックグラウンドで実行中のデリゲート生成タスクの列挙を取得します。
		/// </summary>
		public static IEnumerable<Task> Tasks => TaskList;

		private (Type TargetType, string MethodName, Type ArgumentType)? _MethodCache;
		private Action<object, object?>? _actionCache;
		private Func<object, object?, object>? _funcCache;

		private MethodInfo? _methodInfoCache;

		public object? Invoke( object target, string methodName, object argument ) {
			if( target == null ) throw new ArgumentNullException( nameof( target ) );
			if( methodName == null ) throw new ArgumentNullException( nameof( methodName ) );
			if( argument == null ) throw new ArgumentNullException( nameof( argument ) );

			return Invoke( target, methodName, argument.GetType(), argument );
		}

		/// <summary>
		/// 引数を１つ持つインスタンスメソッドをリフレクションで取得し、呼び出すクラスです。
		/// </summary>
		/// <param name="target">メソッドを呼び出すインスタンス</param>
		/// <param name="methodName">呼び出すメソッドの名前</param>
		/// <param name="argumentType">渡す引数の型</param>
		/// <param name="argument">実際に渡す引数のインスタンス</param>
		/// <returns>呼び出し結果。戻り値が void の場合は null を返します。</returns>
		/// <exception cref="ArgumentNullException">target、methodName、または argumentType が null の場合</exception>
		/// <exception cref="ArgumentException">指定したシグネチャのメソッドが見つからない場合</exception>
		public object? Invoke( object target, string methodName, Type argumentType, object? argument ) {
			if( target == null ) throw new ArgumentNullException( nameof( target ) );
			if( methodName == null ) throw new ArgumentNullException( nameof( methodName ) );
			if( argumentType == null ) throw new ArgumentNullException( nameof( argumentType ) );

			var key = (TargetType: target.GetType(), MethodName: methodName,  ArgumentType: argumentType );
			if( key == _MethodCache ) {
				if( _actionCache != null ) {
					_actionCache( target, argument );
					return null;
				}
				if( _funcCache != null ) {
					return _funcCache( target, argument );
				}

				if( ActionCacheDictionary.TryGetValue( key, out _actionCache ) ) {
					_actionCache( target, argument );
					return null;
				}
				if( FuncCacheDictionary.TryGetValue( key, out _funcCache ) ) {
					return _funcCache( target, argument );
				}

				if( _methodInfoCache != null ) {
					return _methodInfoCache.Invoke( target, [argument] );
				}

				throw new Exception( "Cache Error" );

			} else {
				_MethodCache = key;
				_actionCache = null;
				_funcCache = null;

				if( ActionCacheDictionary.TryGetValue( key, out _actionCache ) ) {
					_actionCache( target, argument );
					return null;
				}
				if( FuncCacheDictionary.TryGetValue( key, out _funcCache ) ) {
					return _funcCache( target, argument );
				}
			}

			_methodInfoCache = key.TargetType
									.GetMethods( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance )
									.Where( method => method.Name == methodName )
									.FirstOrDefault( method => {
										var parameters = method.GetParameters();
										return parameters.Length == 1
												&& ( parameters[0].ParameterType == argumentType || parameters[0].ParameterType.IsAssignableFrom( argumentType ) );
									} );
			if( _methodInfoCache == null ) {
				throw new ArgumentException( $"{key.TargetType.Name} 型に {argumentType.Name} 型の引数を１つ持つメソッド {methodName} が見つかりません。" );
			}

			// キャッシュ処理
			var taskArgs = new { TargetType = key.TargetType, MethodInfo = _methodInfoCache, ParameterType = _methodInfoCache.GetParameters()[0].ParameterType };
			var t = Task.Run( () => {
				if( _methodInfoCache.ReturnType == typeof( void ) ) {
					var paraTarget = Expression.Parameter( typeof( object ), "target" );
					var paraParameterType = Expression.Parameter( typeof( object ), "argument" );

					var method = Expression.Lambda<Action<object, object?>>(
									Expression.Call(
										Expression.Convert( paraTarget, taskArgs.TargetType ),
										taskArgs.MethodInfo,
										Expression.Convert( paraParameterType, taskArgs.ParameterType )
									),
									paraTarget,
									paraParameterType
								).Compile();

					ActionCacheDictionary.TryAdd( (taskArgs.TargetType, taskArgs.MethodInfo.Name, taskArgs.ParameterType), method );
				} else {
					var paraTarget = Expression.Parameter( typeof( object ), "target" );
					var paraParameterType = Expression.Parameter( typeof( object ), "argument" );

					var method = Expression.Lambda<Func<object, object?, object>>(
									Expression.Convert(
										Expression.Call(
											Expression.Convert( paraTarget, taskArgs.TargetType ),
											taskArgs.MethodInfo,
											Expression.Convert( paraParameterType, taskArgs.ParameterType )
										),
										typeof( object )
									),
									paraTarget,
									paraParameterType
								).Compile();

					FuncCacheDictionary.TryAdd( (taskArgs.TargetType, taskArgs.MethodInfo.Name, taskArgs.ParameterType), method );

				}
			} );
			TaskList.Add( t );
			t.ContinueWith( task => {
				TaskList.Remove( task );
			} );

			return _methodInfoCache.Invoke( target, [argument] );
		}
	}
}