using System;

namespace Sylpha {
	/// <summary>
	/// 指定されたリソース解放用の <see cref="Action"/> を <see cref="IDisposable"/> として扱います。
	/// </summary>
	/// <param name="releaseAction">リソースを解放するためのアクション</param>
	public class AnonymousDisposable( Action releaseAction ) : IDisposable {
		private readonly Action _releaseAction = releaseAction ?? throw new ArgumentNullException( nameof( releaseAction ) );

		#region Dispose
		private bool _disposed;

		/// <summary>
		/// コンストラクターで指定されたアクションを呼び出します。
		/// </summary>
		public virtual void Dispose() {
			if( !_disposed ) {
				_disposed = true;

				_releaseAction();
			}
		}
		#endregion
	}
}