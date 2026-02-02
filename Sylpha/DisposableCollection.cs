using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;

namespace Sylpha {
	/// <summary>
	/// 複数のIDisposableオブジェクトをまとめて操作するための機能を提供します。
	/// </summary>
	[PublicAPI]
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
		protected virtual void Dispose( bool disposing ) {
			if( !_disposed ) {
				_disposed = true;

				if( disposing ) {
					foreach( var item in this ) {
						item.Dispose();
					}
				}
			}
		}

		private bool _disposed;

		/// <summary>
		/// このコレクションに含まれるすべての要素をDisposeします。
		/// </summary>
		public void Dispose() {
			Dispose( true );
		}

		#endregion

	}
}