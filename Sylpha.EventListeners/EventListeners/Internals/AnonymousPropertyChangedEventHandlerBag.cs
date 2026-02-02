using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading;

namespace Sylpha.EventListeners.Internals {
	internal class AnonymousPropertyChangedEventHandlerBag : IEnumerable<KeyValuePair<string, List<PropertyChangedEventHandler>>> {
#if NET9_0_OR_GREATER
		private readonly Lock _handlerDictionaryLockObject = new();
		private readonly Dictionary<List<PropertyChangedEventHandler>, Lock> _lockObjectDictionary = [];
#else
		private readonly object _handlerDictionaryLockObject = new();
		private readonly Dictionary<List<PropertyChangedEventHandler>, object> _lockObjectDictionary = [];
#endif

		private readonly Dictionary<string, List<PropertyChangedEventHandler>> _handlerDictionary = [];
		private readonly WeakReference<INotifyPropertyChanged> _source;

		public AnonymousPropertyChangedEventHandlerBag( INotifyPropertyChanged source ) {
			if( source == null ) throw new ArgumentNullException( nameof( source ) );

			_source = new WeakReference<INotifyPropertyChanged>( source );
		}

		IEnumerator<KeyValuePair<string, List<PropertyChangedEventHandler>>> IEnumerable<KeyValuePair<string, List<PropertyChangedEventHandler>>>.GetEnumerator() {
			// ReSharper disable once InconsistentlySynchronizedField
			return _handlerDictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			// ReSharper disable once InconsistentlySynchronizedField
			return _handlerDictionary.GetEnumerator();
		}

		public void RegisterHandler( PropertyChangedEventHandler handler ) {
			RegisterHandler( string.Empty, handler );
		}

		public void RegisterHandler( params IEnumerable<PropertyChangedEventHandler> handlers ) {
			RegisterHandler( string.Empty, handlers );
		}

		public void RegisterHandler( string propertyName, params IEnumerable<PropertyChangedEventHandler> handlers ) {
			lock( _handlerDictionaryLockObject ) {
				if( !_handlerDictionary.TryGetValue( propertyName, out var bag ) ) {
					bag = [];
					_lockObjectDictionary.Add( bag, new() );
					_handlerDictionary[propertyName] = bag;
				}
				bag.AddRange( handlers );
			}
		}

		public void ExecuteHandler( PropertyChangedEventArgs e ) {
			if( e == null ) throw new ArgumentNullException( nameof( e ) );

			if( !_source.TryGetTarget( out var sourceResult ) ) return;

			if( e.PropertyName != null ) {
				List<PropertyChangedEventHandler>? list;
				lock( _handlerDictionaryLockObject ) { _handlerDictionary.TryGetValue( e.PropertyName, out list ); }

				if( list != null ) {
					lock( _lockObjectDictionary[list] ) {
						foreach( var handler in list ) {
							handler( sourceResult, e );
						}
					}
				}
			}

			{
				List<PropertyChangedEventHandler>? allList;
				lock( _handlerDictionaryLockObject ) { _handlerDictionary.TryGetValue( string.Empty, out allList ); }

				if( allList != null ) {
					lock( _lockObjectDictionary[allList] ) {
						foreach( var handler in allList ) {
							handler( sourceResult, e );
						}
					}
				}
			}
		}
	}
}