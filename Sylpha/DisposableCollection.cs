using System;
using System.Collections.ObjectModel;

namespace Sylpha {
	/// <summary>
	/// 複数の <see cref="IDisposable"/> オブジェクトをまとめて操作するための機能を提供します。
	/// </summary>
	public class DisposableCollection : Collection<IDisposable>, IDisposable {
		/// <inheritdoc />
		protected override void InsertItem( int index, IDisposable item ) {
			base.InsertItem( index, item );

			if( _disposed ) {
				item.Dispose();
			}
		}

		/// <inheritdoc />
		protected override void SetItem( int index, IDisposable item ) {
			base.SetItem( index, item );

			if( _disposed ) {
				item.Dispose();
			}
		}

		#region Dispose
		private bool _disposed;

		/// <summary>
		/// コレクション内のすべての <see cref="IDisposable"/> オブジェクトを解放します。
		/// </summary>
		public virtual void Dispose() {
			if( !_disposed ) {
				_disposed = true;

				foreach( var item in this ) {
					item.Dispose();
				}
			}
		}
		#endregion
	}
}