using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Sylpha.Messaging {
	/// <summary>
	/// 引数の無いインスタンスメソッドをリフレクションで取得し、呼び出すクラス
	/// </summary>
	public class MethodBinder {
		private static readonly ConcurrentDictionary<(Type targetType, string methodName), Action<object>> ActionCacheDictionary = [];
		private static readonly ConcurrentDictionary<(Type targetType, string methodName), Func<object, object>> FuncCacheDictionary = [];
		private static readonly List<Task> TaskList = [];

		/// <summary>
		/// キャッシュ中のデリゲート生成タスクの列挙を取得します。
		/// </summary>
		public static IEnumerable<Task> Tasks => TaskList;

		private (Type TargetType, string MethodName)? _MethodCache;

		private Func<object, object>? _functionCache;
		private Action<object>? _actionCache;

		private MethodInfo? _methodInfoCache;

		/// <summary>
		/// 引数の無いインスタンスメソッドをリフレクションで取得し、呼び出すクラス
		/// </summary>
		/// <param name="target">メソッドを呼び出すインスタンス</param>
		/// <param name="methodName">呼び出すメソッドの名前</param>
		/// <returns>呼び出し結果。戻り値が void の場合は null を返します。</returns>
		/// <exception cref="ArgumentNullException">target、または methodName が null の場合</exception>
		/// <exception cref="ArgumentException">指定したシグネチャのメソッドが見つからない場合</exception>
		public object? Invoke( object target, string methodName ) {
			if( target == null ) throw new ArgumentNullException( nameof( target ) );
			if( methodName == null ) throw new ArgumentNullException( nameof( methodName ) );

			var key = (TargetType: target.GetType(), MethodName: methodName);
			if( key == _MethodCache ) {
				if( _actionCache != null ) {
					_actionCache( target );
					return null;
				}
				if( _functionCache != null ) {
					return _functionCache( target );
				}

				if( ActionCacheDictionary.TryGetValue( key, out _actionCache ) ) {
					_actionCache( target );
					return null;
				}
				if( FuncCacheDictionary.TryGetValue( key, out _functionCache ) ) {
					return _functionCache( target );
				}

				if( _methodInfoCache != null ) {
					return _methodInfoCache.Invoke( target, null );
				}

				throw new Exception( "Cache Error" );

			} else {
				_MethodCache = key;
				_actionCache = null;
				_functionCache = null;

				if( ActionCacheDictionary.TryGetValue( key, out _actionCache ) ) {
					_actionCache( target );
					return null;
				}
				if( FuncCacheDictionary.TryGetValue( key, out _functionCache ) ) {
					return _functionCache( target );
				}
			}

			_methodInfoCache = key.TargetType
									.GetMethods( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance )
									.Where( method => method.Name == methodName )
									.FirstOrDefault( method => !method.GetParameters().Any() );
			if( _methodInfoCache == null ) {
				throw new ArgumentException( $"{key.TargetType.Name}型に引数の無いメソッド{methodName}が見つかりません。" );
			}

			// キャッシュ処理
			var taskArgs = new { TargetType = key.TargetType, MethodInfo = _methodInfoCache };
			var t = Task.Run( () => {
				if( taskArgs.MethodInfo.ReturnType == typeof( void ) ) {

					var paraTarget = Expression.Parameter( typeof( object ), "target" );
					var method = Expression.Lambda<Action<object>>(
									Expression.Call(
										Expression.Convert( paraTarget, taskArgs.TargetType ),
										taskArgs.MethodInfo
									),
									paraTarget
								).Compile();

					ActionCacheDictionary.TryAdd( (taskArgs.TargetType, taskArgs.MethodInfo.Name), method );


				} else {
					var paraTarget = Expression.Parameter( typeof( object ), "target" );
					var method = Expression.Lambda<Func<object, object>>(
									Expression.Convert(
										Expression.Call( Expression.Convert( paraTarget, taskArgs.TargetType ), taskArgs.MethodInfo ),
										typeof( object )
									),
									paraTarget
								).Compile();

					FuncCacheDictionary.TryAdd( (taskArgs.TargetType, taskArgs.MethodInfo.Name), method );
				}
			} );

			TaskList.Add( t );
			t.ContinueWith( task => {
				TaskList.Remove( task );
			} );

			return _methodInfoCache.Invoke( target, null );
		}


	}

}