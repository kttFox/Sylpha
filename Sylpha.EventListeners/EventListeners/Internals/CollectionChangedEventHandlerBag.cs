using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;

namespace Sylpha.EventListeners.Internals {
	internal class CollectionChangedEventHandlerBag {
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

		public CollectionChangedEventHandlerBag( INotifyCollectionChanged source ) {
			_source = new WeakReference<INotifyCollectionChanged>( source );
		}

		internal IEnumerator<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>> GetEnumerator() 
			// ReSharper disable once InconsistentlySynchronizedField
			=> _handlerDictionary.GetEnumerator();
			

		public void RegisterHandler( params IEnumerable<NotifyCollectionChangedEventHandler> handlers ) {
			lock( _allHandlerListLockObject ) { _allHandlerList.AddRange( handlers ); }
		}

		public void RegisterHandler( NotifyCollectionChangedAction action, params IEnumerable<NotifyCollectionChangedEventHandler> handlers ) {
			lock( _handlerDictionaryLockObject ) {
				if( !_handlerDictionary.TryGetValue( action, out var bag ) ) {
					bag = [];
					_lockObjectDictionary.Add( bag, new() );
					_handlerDictionary[action] = bag;
				}

				bag.AddRange( handlers );
			}
		}

		public void ExecuteHandler( NotifyCollectionChangedEventArgs e ) {
			if( !_source.TryGetTarget( out var sourceResult ) ) return;

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
				foreach( var handler in _allHandlerList ) {
					handler( sourceResult, e );
				}
			}
		}
	}
}