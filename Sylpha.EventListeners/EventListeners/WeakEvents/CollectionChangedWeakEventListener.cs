using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Sylpha.EventListeners.WeakEvents {
	/// <summary>
	/// <see cref="INotifyCollectionChanged.CollectionChanged"/> を受信するためのWeakイベントリスナーです。
	/// </summary>
	public sealed class CollectionChangedWeakEventListener : WeakEventListener<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>, IEnumerable<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>>, IDisposable {
		private readonly List<NotifyCollectionChangedEventHandler> _allHandlerList = [];
		private readonly Dictionary<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>> _handlerDictionary = [];
		private readonly WeakReference<INotifyCollectionChanged> _source;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">INotifyCollectionChangedオブジェクト</param>
		public CollectionChangedWeakEventListener( INotifyCollectionChanged source ) {
			if( source == null ) throw new ArgumentNullException( nameof( source ) );
			_source = new ( source );

			var _this = this;
			Initialize(
				h => new NotifyCollectionChangedEventHandler( h ),
				h => source.CollectionChanged += h,
				h => source.CollectionChanged -= h,
				( sender, e ) => ExecuteHandler( e )
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
		/// このリスナーインスタンスに新たなハンドラを追加します。
		/// </summary>
		/// <param name="handlers">NotifyCollectionChangedイベントハンドラ</param>
		public void RegisterHandler( params IEnumerable<NotifyCollectionChangedEventHandler> handlers ) {
			if( handlers == null || handlers.Contains( null ) ) throw new ArgumentNullException( nameof( handlers ) );

			ThrowExceptionIfDisposed();
			_allHandlerList.AddRange( handlers );
		}

		/// <summary>
		/// このリスナーインスタンスにプロパティ名でフィルタリング済のハンドラを追加します。
		/// </summary>
		/// <param name="action">ハンドラを登録したいNotifyCollectionChangedAction</param>
		/// <param name="handlers">actionで指定されたNotifyCollectionChangedActionに対応したNotifyCollectionChangedイベントハンドラ</param>
		public void RegisterHandler( NotifyCollectionChangedAction action, params IEnumerable<NotifyCollectionChangedEventHandler> handlers ) {
			if( handlers == null || handlers.Contains( null ) ) throw new ArgumentNullException( nameof( handlers ) );

			ThrowExceptionIfDisposed();
			
			if( !_handlerDictionary.TryGetValue( action, out var list ) ) {
				_handlerDictionary[action] = list = [];
			}
			list.AddRange( handlers );
		}

		public void Add( NotifyCollectionChangedEventHandler handler ) => RegisterHandler( handler );
		public void Add( NotifyCollectionChangedAction action, NotifyCollectionChangedEventHandler handler ) => RegisterHandler( action, handler );
		public void Add( NotifyCollectionChangedAction action, IEnumerable<NotifyCollectionChangedEventHandler> handlers ) => RegisterHandler( action, handlers );
	}
}