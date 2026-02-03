using System;
using System.Collections.Generic;

namespace Sylpha {
	/// <summary>
	/// <see cref="IDisposable"/> の拡張メソッドを提供します。
	/// </summary>
	public static class IDisposableExtensions {
		/// <summary>
		/// <paramref name="disposable"/> を指定したコレクションに追加します。
		/// </summary>
		/// <typeparam name="T"><see cref="IDisposable"/> を実装する型</typeparam>
		/// <param name="disposable">追加する <see cref="IDisposable"/> オブジェクト</param>
		/// <param name="collection">追加先のコレクション</param>
		/// <returns>追加された <paramref name="disposable"/> オブジェクト</returns>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> が <c>null</c> の場合に発生します。</exception>
		public static T AddTo<T>( this T disposable, ICollection<IDisposable> collection ) where T : IDisposable {
			if( collection == null ) {
				throw new ArgumentNullException( nameof( collection ) );
			}
			collection.Add( disposable );

			return disposable;
		}
	}

}