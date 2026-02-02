using System;
using JetBrains.Annotations;

namespace Sylpha.EventListeners.WeakEvents {
	/// <summary>
	/// 汎用WeakEventリスナオブジェクトです。
	/// </summary>
	/// <typeparam name="THandler">対象のイベントのイベントハンドラ型</typeparam>
	/// <typeparam name="TEventArgs">対象のイベントのイベント引数型</typeparam>
	[PublicAPI]
	public class WeakEventListener<THandler, TEventArgs> : IDisposable where TEventArgs : EventArgs {
		private readonly bool _initialized;
		private EventHandler<TEventArgs>? _handler;
		private Action<THandler>? _remove;
		private THandler? _resultHandler;

		protected WeakEventListener() { }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="conversion">ジェネリックイベントハンドラ型をTHandler方に変換するFunc</param>
		/// <param name="add">h => obj.Event += > h の様な形でイベントの購読を登録するためのAction。hはTHandler型です。</param>
		/// <param name="remove">h => obj.Event += > h の様な形でイベントの購読を登録するためのAction。hはTHandler型です。</param>
		/// <param name="handler">イベントを受信した際に行いたいアクション</param>
		public WeakEventListener( Func<EventHandler<TEventArgs>, THandler> conversion, Action<THandler> add, Action<THandler> remove, EventHandler<TEventArgs> handler ) {
			if( conversion == null ) throw new ArgumentNullException( nameof( conversion ) );
			if( add == null ) throw new ArgumentNullException( nameof( add ) );
			if( remove == null ) throw new ArgumentNullException( nameof( remove ) );
			if( handler == null ) throw new ArgumentNullException( nameof( handler ) );

			Initialize( conversion, add, remove, handler );
			_initialized = true;
		}

		private static void ReceiveEvent( WeakReference<WeakEventListener<THandler, TEventArgs>> listenerWeakReference, object? sender, TEventArgs args ) {
			if( listenerWeakReference == null ) throw new ArgumentNullException( nameof( listenerWeakReference ) );

			if( listenerWeakReference.TryGetTarget( out var listenerResult ) ) {
				listenerResult._handler?.Invoke( sender, args );
			}
		}

		private static THandler GetStaticHandler( WeakReference<WeakEventListener<THandler, TEventArgs>> listenerWeakReference, Func<EventHandler<TEventArgs>, THandler> conversion ) {
			if( listenerWeakReference == null ) throw new ArgumentNullException( nameof( listenerWeakReference ) );
			if( conversion == null ) throw new ArgumentNullException( nameof( conversion ) );

			return conversion( ( sender, e ) => ReceiveEvent( listenerWeakReference, sender, e ) );
		}

		protected void Initialize( Func<EventHandler<TEventArgs>, THandler> conversion, Action<THandler> add, Action<THandler> remove, EventHandler<TEventArgs> handler ) {
			if( _initialized ) return;

			if( conversion == null ) throw new ArgumentNullException( nameof( conversion ) );
			if( add == null ) throw new ArgumentNullException( nameof( add ) );

			_handler = handler ?? throw new ArgumentNullException( nameof( handler ) );
			_remove = remove ?? throw new ArgumentNullException( nameof( remove ) );

			_resultHandler = GetStaticHandler( new WeakReference<WeakEventListener<THandler, TEventArgs>>( this ), conversion );

			add( _resultHandler );
		}

		#region Dispose
		protected virtual void Dispose( bool disposing ) {
			if( _disposed ) return;

			if( disposing ) {
				_remove?.Invoke( _resultHandler! );
				_handler = null;
				_resultHandler = default;
				_remove = null;
			}

			_disposed = true;
		}

		private bool _disposed;

		/// <summary>
		/// イベントソースとの接続を解除します。
		/// </summary>
		public void Dispose() {
			Dispose( true );
		}

		protected void ThrowExceptionIfDisposed() {
			if( _disposed ) throw new ObjectDisposedException( "WeakEventListener" );
		}
		#endregion
	}
}