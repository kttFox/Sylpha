using System;
using NUnit.Framework;
using Sylpha.Messaging;

namespace Sylpha.NUnit.Messaging {
	[TestFixture]
	public class MessageListenerTest {
		[Test()]
		public void LifeCycleTest() {
			var listenerSuccess = false;

			var publisher = new Messenger();
			var message = new Message<int>( 1 );

			var listener = new MessageListener( publisher, m => {
				( (Message<int>)m ).Value.Is( 1 );
				listenerSuccess = true;
			} );

			//------------------
			listenerSuccess.Is( false );

			publisher.Raise( message );

			listenerSuccess.Is( true );

			//------------------
			listenerSuccess = false;

			listener.Dispose();
			publisher.Raise( message );

			listenerSuccess.Is( false );

			try {
				listener.RegisterAction( _ => { } );
			} catch( Exception e ) {
				e.GetType().Is( typeof( ObjectDisposedException ) );
			}
		}

		[Test()]
		public void AddHandlerKindTest() {
			var handler1Called = false;
			var handler2Called = false;
			var handler3Called = false;
			var handler4Called = false;
			var handler5Called = false;

			var publisher = new Messenger();
			var message0 = new Message<int>( 1 );
			var message1 = new Message<int>( 1, "Dummy1" );
			var message2 = new Message<int>( 1, "Dummy2" );

			var listener1 = new MessageListener( publisher )
			{
				{"Dummy1", _ => handler1Called = true},
				{"Dummy2",
						_ => handler2Called = true,
						_ => handler3Called = true
				},
				_ => handler4Called = true,
				{"Dummy1", _ => handler5Called = true}
			};

			publisher.Raise( message0 );

			handler1Called.Is( false );
			handler2Called.Is( false );
			handler3Called.Is( false );
			handler4Called.Is( true );
			handler5Called.Is( false );

			handler4Called = false;

			publisher.Raise( message1 );

			handler1Called.Is( true );
			handler2Called.Is( false );
			handler3Called.Is( false );
			handler4Called.Is( true );
			handler5Called.Is( true );

			handler1Called = false;
			handler4Called = false;
			handler5Called = false;

			publisher.Raise( message2 );

			handler1Called.Is( false );
			handler2Called.Is( true );
			handler3Called.Is( true );
			handler4Called.Is( true );
			handler5Called.Is( false );

			handler1Called = false;
			handler2Called = false;
			handler3Called = false;
			handler4Called = false;
			handler5Called = false;

			listener1.Dispose();

			publisher.Raise( message0 );
			publisher.Raise( message1 );
			publisher.Raise( message2 );

			handler1Called.Is( false );
			handler2Called.Is( false );
			handler3Called.Is( false );
			handler4Called.Is( false );
			handler5Called.Is( false );

		}

		[Test()]
		public void RequestMessageTest() {
			var listenerSuccess = false;

			var publisher = new Messenger();
			var message = new RequestMessage<int, string>( 1 );

			var listener = new MessageListener( publisher, m => {
				var rm = (RequestMessage<int, string>)m;
				rm.Value.Is( 1 );
				rm.Response = "test";
				listenerSuccess = true;
			} );

			listenerSuccess.Is( false );

			publisher.GetResponse( message )!.Response.Is( "test" );
			publisher.GetResponse( message )!.Response.Is( "test" );
			listenerSuccess.Is( true );
		}

		[Test()]
		public void SourceReferenceMemoryLeakTest() {
			var handler1Called = false;

			var publisherStrongReference = new Messenger();
			var publisherWeakReference = new WeakReference<Messenger>( publisherStrongReference );
			var message = new Message<int>( 1, "Dummy1" );

			var listener = new MessageListener( publisherStrongReference );
			listener.RegisterAction( "Dummy1", _ => handler1Called = true );

			publisherStrongReference.Raise( message );

			handler1Called.Is( true );

			listener.Dispose();
			publisherStrongReference = null;

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			publisherWeakReference.TryGetTarget( out var resultPublisher ).Is( false );
			resultPublisher.IsNull();
		}
	}
}
