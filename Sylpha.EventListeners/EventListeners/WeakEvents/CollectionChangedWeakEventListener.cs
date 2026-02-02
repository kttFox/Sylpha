using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Sylpha.EventListeners.Internals;

namespace Sylpha.EventListeners.WeakEvents {
	/// <summary>
	/// INotifyCollectionChanged.NotifyCollectionChangedを受信するためのWeakイベントリスナーです。
	/// </summary>
	public sealed class CollectionChangedWeakEventListener : WeakEventListener<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>, IEnumerable<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>>, IDisposable {
		private readonly CollectionChangedEventHandlerBag _bag;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">INotifyCollectionChangedオブジェクト</param>
		public CollectionChangedWeakEventListener( INotifyCollectionChanged source ) {
			if( source == null ) throw new ArgumentNullException( nameof( source ) );

			_bag = new CollectionChangedEventHandlerBag( source );
			Initialize(
				h => new NotifyCollectionChangedEventHandler( h ?? throw new ArgumentNullException( nameof( h ) ) ),
				h => source.CollectionChanged += h,
				h => source.CollectionChanged -= h,
				( sender, e ) => _bag.ExecuteHandler( e ?? throw new ArgumentNullException( nameof( e ) ) )
			);
		}

		IEnumerator<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>> IEnumerable<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>>.GetEnumerator()
			=> _bag.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> _bag.GetEnumerator();

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