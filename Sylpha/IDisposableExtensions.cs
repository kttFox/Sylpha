using System;
using System.Collections.Generic;

namespace Sylpha {
	/// <summary>
	/// Disposableの拡張メソッドを提供します。
	/// </summary>
	public static class IDisposableExtensions {
		/// <summary>
		/// disposableを指定したコレクションに追加します。
		/// </summary>
		/// <param name="disposable"></param>
		/// <param name="collection"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public static T AddTo<T>( this T disposable, ICollection<IDisposable> collection ) where T : IDisposable {
			if( collection == null ) {
				throw new ArgumentNullException( nameof( collection ) );
			}
			collection.Add( disposable );

			return disposable;
		}
	}

}