using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Sylpha.EventListeners.Internals;

namespace Sylpha.EventListeners {
	/// <summary>
	/// INotifyCollectionChanged.NotifyCollectionChangedを受信するためのイベントリスナです。
	/// </summary>
	public sealed class CollectionChangedEventListener : EventListener<NotifyCollectionChangedEventHandler>, IEnumerable<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>>, IDisposable {
		private readonly AnonymousCollectionChangedEventHandlerBag _bag;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">INotifyCollectionChangedオブジェクト</param>
		public CollectionChangedEventListener( INotifyCollectionChanged source ) {
			if( source == null ) throw new ArgumentNullException( nameof( source ) );

			_bag = new AnonymousCollectionChangedEventHandlerBag( source );
			Initialize(
				h => source.CollectionChanged += h,
				h => source.CollectionChanged -= h,
				( sender, e ) => _bag.ExecuteHandler( e )
			);
		}



		IEnumerator<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>> IEnumerable<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>>.GetEnumerator() {
			return ( (IEnumerable<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>>)_bag ).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return ( (IEnumerable)_bag ).GetEnumerator();
		}

		/// <summary>
		/// このリスナーインスタンスに新たなハンドラを追加します。
		/// </summary>
		/// <param name="handler">NotifyCollectionChangedイベントハンドラ</param>
		public void RegisterHandler( NotifyCollectionChangedEventHandler handler ) {
			if( handler == null ) throw new ArgumentNullException( nameof( handler ) );

			ThrowExceptionIfDisposed();
			_bag.RegisterHandler( handler );
		}

		/// <summary>
		/// このリスナーインスタンスにプロパティ名でフィルタリング済のハンドラを追加します。
		/// </summary>
		/// <param name="action">ハンドラを登録したいNotifyCollectionChangedAction</param>
		/// <param name="handlers">actionで指定されたNotifyCollectionChangedActionに対応したNotifyCollectionChangedイベントハンドラ</param>
		public void RegisterHandler( NotifyCollectionChangedAction action, params IEnumerable<NotifyCollectionChangedEventHandler> handlers ) {
			if( handlers == null || handlers.Contains( null ) ) throw new ArgumentNullException( nameof( handlers ) );

			ThrowExceptionIfDisposed();
			_bag.RegisterHandler( action, handlers );
		}

		public void Add( NotifyCollectionChangedEventHandler handler ) => RegisterHandler( handler );
		public void Add( NotifyCollectionChangedAction action, NotifyCollectionChangedEventHandler handler ) => RegisterHandler( action, handler );
		public void Add( NotifyCollectionChangedAction action, IEnumerable<NotifyCollectionChangedEventHandler> handlers ) => RegisterHandler( action, handlers );
	}
}