using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Sylpha.EventListeners.WeakEvents {
	/// <summary>
	/// <see cref="INotifyCollectionChanged.CollectionChanged"/> イベントを受信するためのWeakイベントリスナー
	/// </summary>
	public sealed class CollectionChangedWeakEventListener : WeakEventListener<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>, IEnumerable<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>>, IDisposable {
		private readonly List<NotifyCollectionChangedEventHandler> _allHandlerList = [];
		private readonly Dictionary<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>> _handlerDictionary = [];
		private readonly WeakReference<INotifyCollectionChanged> _source;

		/// <summary>
		/// <see cref="CollectionChangedEventListener"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="source">イベントを受信する対象のオブジェクト</param>
		/// <exception cref="ArgumentNullException"><paramref name="source"/> が <c>null</c> の場合に発生します。</exception>
		public CollectionChangedWeakEventListener( INotifyCollectionChanged source ) {
			if( source == null ) throw new ArgumentNullException( nameof( source ) );
			_source = new( source );

			var _this = this;
			Initialize(
				h => new NotifyCollectionChangedEventHandler( h ),
				h => source.CollectionChanged += h,
				h => source.CollectionChanged -= h,
				( sender, e ) => _this.ExecuteHandler( e )
			);
		}

		void ExecuteHandler( NotifyCollectionChangedEventArgs e ) {
			if( _source.TryGetTarget( out var sourceResult ) ) {
				if( _handlerDictionary.TryGetValue( e.Action, out var list ) ) {
					foreach( var handler in list ) {
						handler( sourceResult, e );
					}
				}

				foreach( var handler in _allHandlerList ) {
					handler( sourceResult, e );
				}
			}
		}

		IEnumerator<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>> IEnumerable<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>>.GetEnumerator()
			=> _handlerDictionary.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> _handlerDictionary.GetEnumerator();

		/// <summary>
		/// 共通のイベント ハンドラーを登録します。
		/// </summary>
		/// <param name="handlers">登録するイベント ハンドラー</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c> 、または <c>null</c> を含む場合に発生します。</exception>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		public void RegisterHandler( params IEnumerable<NotifyCollectionChangedEventHandler> handlers ) {
			if( handlers == null || handlers.Contains( null ) ) throw new ArgumentNullException( nameof( handlers ) );
			ThrowExceptionIfDisposed();

			_allHandlerList.AddRange( handlers );
		}

		/// <summary>
		/// <see cref="NotifyCollectionChangedAction"/>でフィルタリング済のイベント ハンドラーを登録します。
		/// </summary>
		/// <param name="action">イベント ハンドラーを登録したい<see cref="NotifyCollectionChangedAction"/></param>
		/// <param name="handlers">登録するイベント ハンドラー</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c> 、または <c>null</c> を含む場合に発生します。</exception>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		public void RegisterHandler( NotifyCollectionChangedAction action, params IEnumerable<NotifyCollectionChangedEventHandler> handlers ) {
			if( handlers == null || handlers.Contains( null ) ) throw new ArgumentNullException( nameof( handlers ) );
			ThrowExceptionIfDisposed();

			if( !_handlerDictionary.TryGetValue( action, out var list ) ) {
				_handlerDictionary[action] = list = [];
			}
			list.AddRange( handlers );
		}

		/// <summary>
		/// 共通のイベント ハンドラーを登録します。
		/// </summary>
		/// <param name="handler">登録するイベント ハンドラー</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c> 、または <c>null</c> を含む場合に発生します。</exception>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		public void Add( NotifyCollectionChangedEventHandler handler ) => RegisterHandler( handler );
		/// <summary>
		/// <see cref="NotifyCollectionChangedAction"/>でフィルタリング済のイベント ハンドラーを登録します。
		/// </summary>
		/// <param name="action">イベント ハンドラーを登録したい<see cref="NotifyCollectionChangedAction"/></param>
		/// <param name="handler">登録するイベント ハンドラー</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c> 、または <c>null</c> を含む場合に発生します。</exception>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		public void Add( NotifyCollectionChangedAction action, NotifyCollectionChangedEventHandler handler ) => RegisterHandler( action, handler );
		/// <summary>
		/// <see cref="NotifyCollectionChangedAction"/>でフィルタリング済のイベント ハンドラーを登録します。
		/// </summary>
		/// <param name="action">イベント ハンドラーを登録したい<see cref="NotifyCollectionChangedAction"/></param>
		/// <param name="handlers">登録するイベント ハンドラー</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c> 、または <c>null</c> を含む場合に発生します。</exception>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		public void Add( NotifyCollectionChangedAction action, IEnumerable<NotifyCollectionChangedEventHandler> handlers ) => RegisterHandler( action, handlers );
	}
}