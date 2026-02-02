using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Sylpha.EventListeners;
using Sylpha.EventListeners.WeakEvents;

namespace Sylpha.NUnit.EventListeners {
	[TestFixture()]
	public class CollectionChangedWeakEventListenerTest {
		[Test()]
		public void BasicConstructorLifeCycleTest() {
			var listenerSuccess = false;

			void Reset() {
				listenerSuccess = false;
			}

			var publisher = new TestEventPublisher();
			var listener = new CollectionChangedWeakEventListener( publisher ) { 
				( sender, e ) => listenerSuccess = true,
				{ NotifyCollectionChangedAction.Add, (s,e)=>{ } },
				{ NotifyCollectionChangedAction.Add, [(s,e)=>{ }, (s,e)=>{ }] },
			};

			//------------------
			Reset();

			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Add, null );
			listenerSuccess.Is( true );

			//------------------
			Reset();

			listener.Dispose();
			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Add, null );
			listenerSuccess.Is( false );

			//------------------
			try {
				listener.RegisterHandler( ( sender, e ) => listenerSuccess = true );
			} catch( Exception e ) {
				e.GetType().Is( typeof( ObjectDisposedException ) );
			}
		}

		[Test()]
		public void MultipleHandlerLifeCycleTest() {
			var handler1Success = false;
			var handler2Success = false;

			void Reset() {
				handler1Success = false;
				handler2Success = false;
			}

			var publisher = new TestEventPublisher();
			var listener = new CollectionChangedWeakEventListener( publisher );

			//------------------
			Reset();

			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Add, null );

			handler1Success.Is( false );
			handler2Success.Is( false );

			//------------------
			Reset();

			listener.RegisterHandler( ( sender, e ) => handler1Success = true );
			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Add, null );

			handler1Success.Is( true );
			handler2Success.Is( false );

			//------------------
			Reset();

			listener.RegisterHandler( ( sender, e ) => handler2Success = true );
			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Add, null );

			handler1Success.Is( true );
			handler2Success.Is( true );

			//------------------
			Reset();

			listener.Dispose();
			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Add, null );

			handler1Success.Is( false );
			handler2Success.Is( false );
		}

		[Test()]
		public void FilteredHandlerLifeCycleTest() {
			var handler1____Add = false;
			var handler2_Remove = false;
			var handler3____Add = false;

			void Reset() {
				handler1____Add = false;
				handler2_Remove = false;
				handler3____Add = false;
			}

			var publisher = new TestEventPublisher();
			var listener = new CollectionChangedWeakEventListener( publisher );

			//------------------
			Reset();
			listener.RegisterHandler( NotifyCollectionChangedAction.Add, ( sender, e ) => { e.Action.Is( NotifyCollectionChangedAction.Add ); handler1____Add = true; } );
			listener.RegisterHandler( NotifyCollectionChangedAction.Remove, ( sender, e ) => { e.Action.Is( NotifyCollectionChangedAction.Remove ); handler2_Remove = true; } );
			listener.RegisterHandler( NotifyCollectionChangedAction.Add, ( sender, e ) => { e.Action.Is( NotifyCollectionChangedAction.Add ); handler3____Add = true; } );

			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Add, null );
			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Remove, null );

			handler1____Add.Is( true );
			handler2_Remove.Is( true );
			handler3____Add.Is( true );

			//------------------
			Reset();

			listener.Dispose();
			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Add, null );
			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Remove, null );

			handler1____Add.Is( false );
			handler2_Remove.Is( false );
			handler3____Add.Is( false );
		}

		[Test()]
		public void AddHandlerKindTest() {
			var handler1____Add = false;
			var handler2_Remove = false;
			var handler3_Remove = false;
			var handler4_ALL___ = false;
			var handler5____Add = false;

			void Reset() {
				handler1____Add = false;
				handler2_Remove = false;
				handler3_Remove = false;
				handler4_ALL___ = false;
				handler5____Add = false;
			}

			var publisher = new TestEventPublisher();
			var listener1 = new CollectionChangedWeakEventListener( publisher ) {
				{NotifyCollectionChangedAction.Add, (sender, e) => { e.Action.Is(NotifyCollectionChangedAction.Add); handler1____Add = true; }},
				{NotifyCollectionChangedAction.Remove,[
						(sender, e) => { e.Action.Is(NotifyCollectionChangedAction.Remove); handler2_Remove = true;},
						(sender, e) => { e.Action.Is(NotifyCollectionChangedAction.Remove); handler3_Remove = true;}
				]},
				(sender,e) => handler4_ALL___ = true,
				{NotifyCollectionChangedAction.Add, (sender, e) => { e.Action.Is(NotifyCollectionChangedAction.Add); handler5____Add = true; }}
			};

			//-------------------------------
			Reset();
			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Reset, null );

			handler1____Add.Is( false );
			handler2_Remove.Is( false );
			handler3_Remove.Is( false );
			handler4_ALL___.Is( true );
			handler5____Add.Is( false );

			//-------------------------------
			Reset();

			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Add, null );

			handler1____Add.Is( true );
			handler2_Remove.Is( false );
			handler3_Remove.Is( false );
			handler4_ALL___.Is( true );
			handler5____Add.Is( true );

			//-------------------------------
			Reset();

			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Remove, null );

			handler1____Add.Is( false );
			handler2_Remove.Is( true );
			handler3_Remove.Is( true );
			handler4_ALL___.Is( true );
			handler5____Add.Is( false );

			//-------------------------------
			Reset();

			listener1.Dispose();

			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Add, null );
			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Remove, null );

			handler1____Add.Is( false );
			handler2_Remove.Is( false );
			handler3_Remove.Is( false );
			handler4_ALL___.Is( false );
			handler5____Add.Is( false );

		}

		/// <summary>
		/// パブリッシャー(<see cref="TestEventPublisher"/>)への参照がメモリリークしないことを検証するテスト。
		/// リスナー(<see cref="CollectionChangedWeakEventListener"/>)を破棄した後、パブリッシャーへの強参照を解放すると、
		/// パブリッシャーがGCによって回収されることを確認します。
		/// </summary>
		[Test()]
		public void PublisherWeakReferenceReleaseTest() {
			var publisher = new PublisherLifetimeTestHelper();
			publisher.Listener?.Dispose();

			publisher.ClearPublisher();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			// TestEventPublisherのインスタンスが解放されていることを確認する
			publisher.PublisherIsAlive().Is( false );
		}

		/// <summary>
		/// パブリッシャー(<see cref="TestEventPublisher"/>)への参照が'''メモリリークする'''ことを検証するテスト。
		/// リスナー(<see cref="CollectionChangedWeakEventListener"/>)を破棄しない場合、パブリッシャーへの強参照が残る。
		/// </summary>
		[Test()]
		public void PublisherWeakReferenceReleaseTest_WithoutDispose() {
			var publisher = new PublisherLifetimeTestHelper();
			//publisher.Listener.Dispose(); // Disposeしないので参照が残る

			publisher.ClearPublisher();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			// TestEventPublisherのインスタンスが解放さない
			publisher.PublisherIsAlive().Is( true );
		}

		class PublisherLifetimeTestHelper {
			TestEventPublisher? Publisher;
			readonly WeakReference<TestEventPublisher> WeakReference;

			public CollectionChangedWeakEventListener Listener { get; }

			public PublisherLifetimeTestHelper() {
				Publisher = new TestEventPublisher();
				WeakReference = new( Publisher );

				var handler1Success = false;
				Listener = new CollectionChangedWeakEventListener( Publisher ) { ( sender, e ) => handler1Success = true };
				Publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Add, null );
				handler1Success.Is( true );
			}

			public bool PublisherIsAlive() => this.WeakReference.TryGetTarget( out var _ );
			public void ClearPublisher ()=> Publisher = null;
		}

		/// <summary>
		/// リスナー(<see cref="CollectionChangedWeakEventListener"/>)への参照がメモリリークしないことを検証するテスト。
		/// </summary>
		[Test()]
		public void ListenerWeakReferenceReleaseTest() {
			var publisher = new TestEventPublisher();
			var listenerWeakReference = new ListenerLifetimeTestHelper( publisher );

			//------------------
			listenerWeakReference.Reset();

			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Add, null );
			listenerWeakReference.Success.Is( true );

			//------------------
			listenerWeakReference.Dispose();
			listenerWeakReference.ClearListener();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			//-----------------------------
			listenerWeakReference.Reset();

			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Add, null );

			listenerWeakReference.ListenerIsAlive().Is( false );
			listenerWeakReference.Success.Is( false );
		}

		/// <summary>
		/// リスナー(<see cref="CollectionChangedWeakEventListener"/>)への参照がメモリリークしないことを検証するテスト。(Dispose未呼び出し)
		/// </summary>
		[Test()]
		public void ListenerWeakReferenceReleaseTest_WithoutDispose() {
			var publisher = new TestEventPublisher();
			var listenerWeakReference = new ListenerLifetimeTestHelper( publisher );

			//------------------
			listenerWeakReference.Reset();

			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Add, null );
			listenerWeakReference.Success.Is( true );

			//------------------
			//listenerWeakReference.Dispose();
			listenerWeakReference.ClearListener();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			//-----------------------------
			listenerWeakReference.Reset();

			publisher.RaiseCollectionChanged( NotifyCollectionChangedAction.Add, null );

			listenerWeakReference.ListenerIsAlive().Is( false );
			listenerWeakReference.Success.Is( false );
		}

		class ListenerLifetimeTestHelper {
			CollectionChangedWeakEventListener? Listener;

			readonly WeakReference<CollectionChangedWeakEventListener>? WeakReference;

			public ListenerLifetimeTestHelper( INotifyCollectionChanged notifyCollection ) {
				Listener = new CollectionChangedWeakEventListener( notifyCollection ) { ( sender, e ) => Success = true };
				WeakReference = new( Listener );
			}

			public bool ListenerIsAlive() => WeakReference?.TryGetTarget( out var _ ) == true;
			public bool Success { get; private set; }
			public void Reset() => Success = false;
			public void ClearListener() => Listener = null;
			public void Dispose() => Listener?.Dispose();
		}
	}
}
