using System;
using System.Collections.Specialized;
using System.ComponentModel;
using NUnit.Framework;
using Sylpha.EventListeners.WeakEvents;
using Sylpha.Messaging;

namespace Sylpha.NUnit.Messaging {
	[TestFixture]
	public class MessageListenerTest {
		[Test()]
		public void LifeCycleTest() {
			var listenerSuccess = false;
			void Reset() {
				listenerSuccess = false;
			}
			var publisher = new Messenger();
			var message = new Message<int>( 1 );

			var listener = new MessageListener( publisher ){
				m => {
					( (Message<int>)m ).Value.Is( 1 );
					listenerSuccess = true;
				}
			};

			//------------------
			Reset();

			publisher.Raise( message );

			listenerSuccess.Is( true );

			//------------------
			Reset();

			listener.Dispose();
			publisher.Raise( message );

			listenerSuccess.Is( false );

			//------------------
			try {
				listener.RegisterAction( _ => { } );
			} catch( Exception e ) {
				e.GetType().Is( typeof( ObjectDisposedException ) );
			}
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

			var publisher = new Messenger();
			var message = new Message<int>( 1 );
			var messageDummy1 = new Message<int>( 1, "Dummy1" );
			var messageDummy2 = new Message<int>( 1, "Dummy2" );

			var listener1 = new MessageListener( publisher ) {
				{"Dummy1", _ => handler1_Dummy1 = true},
				{"Dummy2",[
						_ => handler2_Dummy2 = true,
						_ => handler3_Dummy2 = true
				]},
				_ => handler4_ALL___ = true,
				{"Dummy1", _ => handler5_Dummy1 = true}
			};

			//------------------------
			Reset();

			publisher.Raise( message );

			handler1_Dummy1.Is( false );
			handler2_Dummy2.Is( false );
			handler3_Dummy2.Is( false );
			handler4_ALL___.Is( true );
			handler5_Dummy1.Is( false );

			//------------------------
			Reset();

			publisher.Raise( messageDummy1 );

			handler1_Dummy1.Is( true );
			handler2_Dummy2.Is( false );
			handler3_Dummy2.Is( false );
			handler4_ALL___.Is( true );
			handler5_Dummy1.Is( true );

			//------------------------
			Reset();

			publisher.Raise( messageDummy2 );

			handler1_Dummy1.Is( false );
			handler2_Dummy2.Is( true );
			handler3_Dummy2.Is( true );
			handler4_ALL___.Is( true );
			handler5_Dummy1.Is( false );

			//------------------------
			Reset();

			listener1.Dispose();

			publisher.Raise( message );
			publisher.Raise( messageDummy1 );
			publisher.Raise( messageDummy2 );

			handler1_Dummy1.Is( false );
			handler2_Dummy2.Is( false );
			handler3_Dummy2.Is( false );
			handler4_ALL___.Is( false );
			handler5_Dummy1.Is( false );
		}

		[Test()]
		public void RequestMessageTest() {
			var listenerSuccess = false;

			var publisher = new Messenger();
			var message = new RequestMessage<int, string>( 1 );

			var listener = new MessageListener( publisher ){
				m => {
					var rm = (RequestMessage<int, string>)m;
					rm.Value.Is( 1 );
					rm.Response = "test";
					listenerSuccess = true;
				}
			};

			publisher.Raise( message ).Response.Is( "test" );
			publisher.Raise( message ).Response.Is( "test" );
			listenerSuccess.Is( true );
		}

		/// <summary>
		/// パブリッシャー(<see cref="Messenger"/>)への参照がメモリリークしないことを検証するテスト。
		/// リスナー(<see cref="MessageListener"/>)を破棄した後、パブリッシャーへの強参照を解放すると、
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
		/// パブリッシャー(<see cref="Messenger"/>)への参照が'''メモリリークする'''ことを検証するテスト。
		/// リスナー(<see cref="MessageListener"/>)を破棄しない場合、パブリッシャーへの強参照が残る。
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
			Messenger? Publisher;
			readonly WeakReference<Messenger> WeakReference;

			public MessageListener Listener { get; }

			public PublisherLifetimeTestHelper() {
				Publisher = new Messenger();
				WeakReference = new( Publisher );

				var handler1Success = false;
				Listener = new MessageListener( Publisher ) { _ => handler1Success = true };
				Publisher.Raise( new Message<int>( 1, "Dummy1" ) );
				handler1Success.Is( true );
			}

			public bool PublisherIsAlive() => this.WeakReference.TryGetTarget( out var _ );
			public void ClearPublisher() => Publisher = null;
		}

	}
}
