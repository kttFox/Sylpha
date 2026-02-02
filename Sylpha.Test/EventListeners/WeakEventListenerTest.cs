using System;
using System.Collections.Specialized;
using NUnit.Framework;
using Sylpha.EventListeners.WeakEvents;

namespace Sylpha.NUnit.EventListeners {
	[TestFixture()]
	public class WeakEventListenerTest {
		[Test()]
		public void LifeCycleTest() {
			var listenerSuccess = false;

			void Reset() {
				listenerSuccess = false;
			}

			var publisher = new TestEventPublisher();
			var listener = new WeakEventListener<EventHandler, EventArgs>(
				h => new EventHandler( h ),
				h => publisher.EmptyEvent += h,
				h => publisher.EmptyEvent -= h,
				( sender, e ) => listenerSuccess = true 
			);
			//------------------
			Reset();

			publisher.RaiseEmptyEvent();
			listenerSuccess.Is( true );

			//------------------
			Reset();

			listener.Dispose();
			publisher.RaiseEmptyEvent();
			listenerSuccess.Is( false );
			
		}

		/// <summary>
		/// パブリッシャー(<see cref="TestEventPublisher"/>)への参照がメモリリークしないことを検証するテスト。
		/// リスナー(<see cref="WeakEventListener{T,T}"/>)を破棄した後、パブリッシャーへの強参照を解放すると、
		/// パブリッシャーがGCによって回収されることを確認します。
		/// </summary>
		[Test()]
		public void PublisherWeakReferenceReleaseTest() {
			var publisher = new PublisherLifetimeTestHelper();
			publisher.Listener.Dispose();

			publisher.ClearPublisher();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			// TestEventPublisherのインスタンスが解放されていることを確認する
			publisher.PublisherIsAlive().Is( false );
		}


		/// <summary>
		/// パブリッシャー(<see cref="TestEventPublisher"/>)への参照が'''メモリリークする'''ことを検証するテスト。
		/// リスナー(<see cref="WeakEventListener{T,T}"/>)を破棄しない場合、パブリッシャーへの強参照が残る。
		/// </summary>
		[Test()]
		public void PublisherWeakReferenceReleaseTest_WithoutDispose() {
			var publisher = new PublisherLifetimeTestHelper();
			//publisher.Listener.Dispose();	// Disposeしないので参照が残る

			publisher.ClearPublisher();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			// Publisherのインスタンスが解放されない
			publisher.PublisherIsAlive().Is( true );
		}

		class PublisherLifetimeTestHelper {
			TestEventPublisher? Publisher;
			readonly WeakReference<TestEventPublisher> WeakReference;

			public WeakEventListener<EventHandler, EventArgs> Listener { get; }

			public PublisherLifetimeTestHelper() {
				Publisher = new TestEventPublisher();
				WeakReference = new( Publisher );

				var publisher = Publisher; // ローカル変数で受ける
				var handler1Success = false;
				Listener = new WeakEventListener<EventHandler, EventArgs>(
					h => new EventHandler( h ),
					h => publisher.EmptyEvent += h,
					h => publisher.EmptyEvent -= h,
					( sender, e ) => {
						handler1Success = true;
					}
				);

				Publisher.RaiseEmptyEvent();
				handler1Success.Is( true );
			}
			public bool PublisherIsAlive() => this.WeakReference.TryGetTarget( out var _ );
			public void ClearPublisher() => Publisher = null;
		}

		/// <summary>
		/// リスナー(<see cref="WeakEventListener{T,T}"/>)への参照がメモリリークしないことを検証するテスト。
		/// </summary>
		[Test()]
		public void ListenerWeakReferenceReleaseTest() {
			var publisher = new TestEventPublisher();
			var listenerWeakReference = new ListenerLifetimeTestHelper( publisher );

			//------------------
			listenerWeakReference.Reset();

			publisher.RaiseEmptyEvent();
			listenerWeakReference.Success.Is( true );

			//------------------
			listenerWeakReference.Dispose();
			listenerWeakReference.ClearListener();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			//------------------
			listenerWeakReference.Reset();

			publisher.RaiseEmptyEvent();

			listenerWeakReference.ListenerIsAlive().Is( false );
			listenerWeakReference.Success.Is( false );
		}

		/// <summary>
		/// リスナー(<see cref="CollectionChangedEventListener"/>)への参照がメモリリークしないことを検証するテスト。(Dispose未呼び出し)
		/// </summary>
		[Test()]
		public void ListenerWeakReferenceReleaseTest_WithoutDispose() {
			var publisher = new TestEventPublisher();
			var listenerWeakReference = new ListenerLifetimeTestHelper( publisher );

			//------------------
			listenerWeakReference.Reset();

			publisher.RaiseEmptyEvent();
			listenerWeakReference.Success.Is( true );

			//------------------
			//listenerWeakReference.Dispose();
			listenerWeakReference.ClearListener();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			//------------------
			listenerWeakReference.Reset();

			publisher.RaiseEmptyEvent();

			listenerWeakReference.ListenerIsAlive().Is( false );
			listenerWeakReference.Success.Is( false );
		}

		class ListenerLifetimeTestHelper {
			WeakEventListener<EventHandler, EventArgs>? Listener;

			readonly WeakReference<WeakEventListener<EventHandler, EventArgs>> WeakReference;

			public ListenerLifetimeTestHelper( TestEventPublisher testEventPublisher ) {
				Listener = new WeakEventListener<EventHandler, EventArgs>(
					h => new EventHandler( h ),
					h => testEventPublisher.EmptyEvent += h,
					h => testEventPublisher.EmptyEvent -= h,
					( sender, e ) => Success = true
				);

				WeakReference = new( Listener );
			}

			public bool ListenerIsAlive() => this.WeakReference.TryGetTarget( out var _ );
			public bool Success { get; private set; }
			public void Reset() => Success = false;
			public void ClearListener() => Listener = null;
			public void Dispose() => Listener?.Dispose();
		}
	}
}
