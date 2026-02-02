using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Sylpha.EventListeners;
using Sylpha.EventListeners.WeakEvents;

namespace Sylpha.NUnit.EventListeners {
	[TestFixture()]
	public class PropertyChangedEventListenerTest {
		[Test()]
		public void BasicConstructorLifeCycleTest() {
			var listenerSuccess = false;

			void Reset() {
				listenerSuccess = false;
			}

			var publisher = new NUnit.TestEventPublisher();
			var listener = new PropertyChangedEventListener( publisher ) {
				( sender, e ) => listenerSuccess = true,
				{ "Dummy", (s,e)=>{ } },
				{ "Dummy", [(s,e)=>{ }, (s,e)=>{ }] },
				{ ()=> publisher.Dummy2, (s,e)=>{ } },
				{ ()=> publisher.Dummy2, [(s,e)=>{ }, (s,e)=>{ }] },
			};



			//------------------
			Reset();

			listenerSuccess.Is( false );
			publisher.RaisePropertyChanged( string.Empty );
			listenerSuccess.Is( true );

			//------------------
			Reset();

			listener.Dispose();
			publisher.RaisePropertyChanged( string.Empty );
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

			var publisher = new NUnit.TestEventPublisher();
			var listener = new PropertyChangedEventListener( publisher );

			//------------------
			Reset();

			publisher.RaisePropertyChanged( string.Empty );

			handler1Success.Is( false );
			handler2Success.Is( false );

			//------------------
			Reset();

			listener.RegisterHandler( ( sender, e ) => handler1Success = true );
			publisher.RaisePropertyChanged( string.Empty );

			handler1Success.Is( true );
			handler2Success.Is( false );

			//------------------
			Reset();

			listener.RegisterHandler( ( sender, e ) => handler2Success = true );
			publisher.RaisePropertyChanged( string.Empty );

			handler1Success.Is( true );
			handler2Success.Is( true );

			//------------------
			Reset();

			listener.Dispose();
			publisher.RaisePropertyChanged( string.Empty );

			handler1Success.Is( false );
			handler2Success.Is( false );
		}

		[Test()]
		public void FilteredHandlerLifeCycleTest() {
			var handler1_Dummy1 = false;
			var handler2_Dummy2 = false;
			var handler3_Dummy1 = false;

			void Reset() {
				handler1_Dummy1 = false;
				handler2_Dummy2 = false;
				handler3_Dummy1 = false;
			}

			var publisher = new NUnit.TestEventPublisher();
			var listener = new PropertyChangedEventListener( publisher );

			//------------------
			Reset();

			listener.RegisterHandler( "Dummy1", ( sender, e ) => { e.PropertyName.Is( "Dummy1" ); handler1_Dummy1 = true; } );
			listener.RegisterHandler( "Dummy2", ( sender, e ) => { e.PropertyName.Is( "Dummy2" ); handler2_Dummy2 = true; } );
			listener.RegisterHandler( "Dummy1", ( sender, e ) => { e.PropertyName.Is( "Dummy1" ); handler3_Dummy1 = true; } );

			publisher.RaisePropertyChanged( "Dummy1" );
			publisher.RaisePropertyChanged( "Dummy2" );

			handler1_Dummy1.Is( true );
			handler2_Dummy2.Is( true );
			handler3_Dummy1.Is( true );

			//------------------
			Reset();

			listener.Dispose();
			publisher.RaisePropertyChanged( "Dummy1" );
			publisher.RaisePropertyChanged( "Dummy2" );

			handler1_Dummy1.Is( false );
			handler2_Dummy2.Is( false );
			handler3_Dummy1.Is( false );
		}

		[Test()]
		public void AddHandlerKindTest() {
			var handler1_Dummy1 = false;
			var handler2_Dummy2 = false;
			var handler3_Dummy2 = false;
			var handler4_ALL___ = false;
			var handler5_Dummy1 = false;

			void Reset() {
				handler1_Dummy1 = false;
				handler2_Dummy2 = false;
				handler3_Dummy2 = false;
				handler4_ALL___ = false;
				handler5_Dummy1 = false;
			}

			var publisher = new NUnit.TestEventPublisher();
			var listener1 = new PropertyChangedEventListener( publisher ) {
				{"Dummy1", (sender, e) => { e.PropertyName.Is("Dummy1"); handler1_Dummy1 = true; }},
				{() => publisher.Dummy2, [
						(sender, e) => { e.PropertyName.Is("Dummy2"); handler2_Dummy2 = true;},
						(sender, e) => { e.PropertyName.Is("Dummy2"); handler3_Dummy2 = true;}
				]},
				(sender,e) => handler4_ALL___ = true,
				{"Dummy1", (sender, e) => { e.PropertyName.Is("Dummy1"); handler5_Dummy1 = true; }}
			};

			//------------------
			Reset();
			publisher.RaisePropertyChanged( null );

			handler1_Dummy1.Is( false );
			handler2_Dummy2.Is( false );
			handler3_Dummy2.Is( false );
			handler4_ALL___.Is( true );
			handler5_Dummy1.Is( false );

			//------------------
			Reset();

			publisher.RaisePropertyChanged( "Dummy1" );

			handler1_Dummy1.Is( true );
			handler2_Dummy2.Is( false );
			handler3_Dummy2.Is( false );
			handler4_ALL___.Is( true );
			handler5_Dummy1.Is( true );

			//------------------
			Reset();

			publisher.RaisePropertyChanged( "Dummy2" );

			handler1_Dummy1.Is( false );
			handler2_Dummy2.Is( true );
			handler3_Dummy2.Is( true );
			handler4_ALL___.Is( true );
			handler5_Dummy1.Is( false );

			//------------------
			Reset();

			listener1.Dispose();

			publisher.RaisePropertyChanged( "Dummy1" );
			publisher.RaisePropertyChanged( "Dummy2" );

			handler1_Dummy1.Is( false );
			handler2_Dummy2.Is( false );
			handler3_Dummy2.Is( false );
			handler4_ALL___.Is( false );
			handler5_Dummy1.Is( false );

		}

		/// <summary>
		/// パブリッシャー(<see cref="TestEventPublisher"/>)への参照がメモリリークしないことを検証するテスト。
		/// リスナー(<see cref="PropertyChangedEventListener"/>)を破棄した後、パブリッシャーへの強参照を解放すると、
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
		/// リスナー(<see cref="PropertyChangedEventListener"/>)を破棄しない場合、パブリッシャーへの強参照が残る。
		/// </summary>
		[Test()]
		public void PublisherWeakReferenceReleaseTest_WithoutDispose() {
			var publisher = new PublisherLifetimeTestHelper();
			//publisher.Listener.Dispose();	// Disposeしないので参照が残る

			publisher.ClearPublisher();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			// TestEventPublisherのインスタンスが解放されない
			publisher.PublisherIsAlive().Is( true );
		}

		class PublisherLifetimeTestHelper {
			TestEventPublisher? Publisher;
			readonly WeakReference<TestEventPublisher> WeakReference;

			public PropertyChangedEventListener Listener { get; }

			public PublisherLifetimeTestHelper() {
				Publisher = new TestEventPublisher();
				WeakReference = new( Publisher );

				var handler1Success = false;
				Listener = new PropertyChangedEventListener( Publisher ) {
					{ "Dummy1",
						( sender,e ) => {
							e.PropertyName.Is( "Dummy1" );
							handler1Success = true;
						}
					}
				};
				Publisher.RaisePropertyChanged( "Dummy1" );
				handler1Success.Is( true );
			}
			public bool PublisherIsAlive() => this.WeakReference.TryGetTarget( out var _ );
			public void ClearPublisher() => Publisher = null;
		}

		/// <summary>
		/// リスナー(<see cref="PropertyChangedEventListener"/>)への参照がメモリリークしないことを検証するテスト。
		/// </summary>
		[Test()]
		public void ListenerWeakReferenceReleaseTest() {
			var publisher = new TestEventPublisher();
			var listenerWeakReference = new ListenerLifetimeTestHelper( publisher );

			//------------------
			listenerWeakReference.Reset();

			publisher.RaisePropertyChanged( null );
			listenerWeakReference.Success.Is( true );

			//------------------
			listenerWeakReference.Dispose();
			listenerWeakReference.ClearListener();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			//------------------
			listenerWeakReference.Reset();

			publisher.RaisePropertyChanged( null );

			listenerWeakReference.ListenerIsAlive().Is( false );
			listenerWeakReference.Success.Is( false );
		}

		/// <summary>
		/// リスナー(<see cref="PropertyChangedEventListener"/>)への参照がメモリリーク"する"ことを検証するテスト。(Dispose未呼び出し)
		/// </summary>
		[Test()]
		public void ListenerWeakReferenceReleaseTest_WithoutDispose() {
			var publisher = new TestEventPublisher();
			var listenerWeakReference = new ListenerLifetimeTestHelper( publisher );

			//------------------
			listenerWeakReference.Reset();

			publisher.RaisePropertyChanged( null );
			listenerWeakReference.Success.Is( true );

			//------------------
			//listenerWeakReference.Dispose();
			listenerWeakReference.ClearListener();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			//------------------
			listenerWeakReference.Reset();

			publisher.RaisePropertyChanged( null );

			listenerWeakReference.ListenerIsAlive().Is( true );
			listenerWeakReference.Success.Is( true );
		}

		class ListenerLifetimeTestHelper {
			public PropertyChangedEventListener? Listener { get; private set; }

			readonly WeakReference<PropertyChangedEventListener>? WeakReference;

			public ListenerLifetimeTestHelper( INotifyPropertyChanged notifyProperty ) {
				Listener = new PropertyChangedEventListener( notifyProperty ) { ( sender, e ) => Success = true };
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
