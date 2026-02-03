using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sylpha {
	/// <summary>
	/// 複数のIDisposableオブジェクトをまとめて操作するための機能を提供します。
	/// </summary>
	public class DisposableCollection : Collection<IDisposable>, IDisposable {
		protected override void InsertItem( int index, IDisposable item ) {
			base.InsertItem( index, item );

			if( _disposed ) {
				item.Dispose();
			}
		}

		protected override void SetItem( int index, IDisposable item ) {
			base.SetItem( index, item );

			if( _disposed ) {
				item.Dispose();
			}
		}

		#region Dispose
		private bool _disposed;

		/// <summary>
		/// このコレクションに含まれるすべての要素をDisposeします。
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