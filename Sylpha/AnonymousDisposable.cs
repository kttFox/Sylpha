using System;

namespace Sylpha {
	/// <summary>
	/// 指定されたリソース解放用のActionをIDisposableとして扱います。
	/// </summary>
	public class AnonymousDisposable : IDisposable {
		private readonly Action _releaseAction;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="releaseAction">リソースを解放するためのアクション</param>
		public AnonymousDisposable( Action releaseAction ) {
			_releaseAction = releaseAction ?? throw new ArgumentNullException( nameof( releaseAction ) );
		}

		#region Dispose
		private bool _disposed;

		/// <summary>
		/// コンストラクタで指定されたアクションを呼び出します。
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