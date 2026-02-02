using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Sylpha.EventListeners {
	/// <summary>
	/// 汎用イベントリスナオブジェクトです。
	/// </summary>
	/// <typeparam name="THandler">イベントハンドラーの型</typeparam>
	[PublicAPI]
	public class EventListener<THandler> : IDisposable where THandler : class {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="add">h => obj.Event += > h の様な形でイベントの購読を登録するためのAction。hはTHandler型です。</param>
		/// <param name="remove">h => obj.Event -= > h の様な形でイベントの購読を解除するためのAction。hはTHandler型です。</param>
		/// <param name="handler">イベントを受信した際に行いたいアクション</param>
		public EventListener( Action<THandler> add, Action<THandler> remove, THandler handler ) {
			if( add == null ) throw new ArgumentNullException( nameof( add ) );
			if( remove == null ) throw new ArgumentNullException( nameof( remove ) );
			if( handler == null ) throw new ArgumentNullException( nameof( handler ) );

			Initialize( add, remove, handler );
		}

		protected EventListener() { }
		

		private bool _initialized;

		private Action<THandler>? _remove;
		private THandler? _handler;

		[MemberNotNull( nameof( _remove ), nameof( _handler ) )]
		protected void Initialize( Action<THandler> add, Action<THandler> remove, THandler handler ) {
			if( _initialized ) {
				throw new Exception( "すでに初期化済みです。" );
			}

			if( add == null ) throw new ArgumentNullException( nameof( add ) );
			_handler = handler ?? throw new ArgumentNullException( nameof( handler ) );
			_remove = remove ?? throw new ArgumentNullException( nameof( remove ) );

			add( handler );
			_initialized = true;
		}

		#region Dispose
		protected virtual void Dispose( bool disposing ) {
			if( !_disposed ) {
				_disposed = true;

				if( disposing ) {
					_remove?.Invoke( _handler! );
				}
			}
		}

		private bool _disposed;

		/// <summary>
		/// イベントハンドラの登録を解除します。
		/// </summary>
		public void Dispose() {
			Dispose( true );
		}
		protected void ThrowExceptionIfDisposed() {
			if( _disposed ) throw new ObjectDisposedException( "EventListener" );
		}

		#endregion
	}
}