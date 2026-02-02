using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;

namespace Sylpha.EventListeners.Internals {
	internal class AnonymousCollectionChangedEventHandlerBag : IEnumerable<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>> {
#if NET9_0_OR_GREATER
		private readonly Lock _allHandlerListLockObject = new();
		private readonly Lock _handlerDictionaryLockObject = new();
		private readonly Dictionary<List<NotifyCollectionChangedEventHandler>, Lock> _lockObjectDictionary = [];
#else
		private readonly object _allHandlerListLockObject = new();
		private readonly object _handlerDictionaryLockObject = new();
		private readonly Dictionary<List<NotifyCollectionChangedEventHandler>, object> _lockObjectDictionary = [];
#endif

		private readonly List<NotifyCollectionChangedEventHandler> _allHandlerList = [];
		private readonly Dictionary<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>> _handlerDictionary = [];
		private readonly WeakReference<INotifyCollectionChanged> _source;

		public AnonymousCollectionChangedEventHandlerBag( INotifyCollectionChanged source ) {
			if( source == null ) throw new ArgumentNullException( nameof( source ) );

			_source = new WeakReference<INotifyCollectionChanged>( source );
		}

		public AnonymousCollectionChangedEventHandlerBag( INotifyCollectionChanged source, NotifyCollectionChangedEventHandler handler ) : this( source ) {
			if( handler == null ) throw new ArgumentNullException( nameof( handler ) );

			RegisterHandler( handler );
		}

		IEnumerator<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>> IEnumerable<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>>.GetEnumerator() {
			// ReSharper disable once InconsistentlySynchronizedField
			return _handlerDictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			// ReSharper disable once InconsistentlySynchronizedField
			return _handlerDictionary.GetEnumerator();
		}

		public void RegisterHandler( NotifyCollectionChangedEventHandler handler ) {
			lock( _allHandlerListLockObject ) { _allHandlerList.Add( handler ); }
		}

		public void RegisterHandler( NotifyCollectionChangedAction action, NotifyCollectionChangedEventHandler handler ) {
			lock( _handlerDictionaryLockObject ) {
				if( !_handlerDictionary.TryGetValue( action, out var bag ) ) {
					bag = [];
					_lockObjectDictionary.Add( bag, new() );
					_handlerDictionary[action] = bag;
				}

				bag.Add( handler );
			}
		}

		public void ExecuteHandler( NotifyCollectionChangedEventArgs e ) {
			if( e == null ) throw new ArgumentNullException( nameof( e ) );

			if( _source.TryGetTarget( out var sourceResult ) ) {
				List<NotifyCollectionChangedEventHandler>? list;
				lock( _handlerDictionaryLockObject ) { _handlerDictionary.TryGetValue( e.Action, out list ); }

				if( list != null ) {
					lock( _lockObjectDictionary[list] ) {
						foreach( var handler in list ) {
							handler( sourceResult, e );
						}
					}
				}

				lock( _allHandlerListLockObject ) {
					if( !_allHandlerList.Any() ) return;

					foreach( var handler in _allHandlerList ) {
						handler( sourceResult, e );
					}
				}
			}
		}

		public void Add( NotifyCollectionChangedEventHandler handler ) {
			RegisterHandler( handler );
		}

		public void Add( NotifyCollectionChangedAction action, NotifyCollectionChangedEventHandler handler ) {
			RegisterHandler( action, handler );
		}

		public void Add( NotifyCollectionChangedAction action, params NotifyCollectionChangedEventHandler[] handlers ) {
			if( handlers == null ) throw new ArgumentNullException( nameof( handlers ) );

			foreach( var handler in handlers ) {
				RegisterHandler( action, handler );
			}
		}
	}
}