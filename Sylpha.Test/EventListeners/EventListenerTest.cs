using Sylpha.EventListeners;
using NUnit.Framework;
using System;

namespace Sylpha.NUnit.EventListeners {
	[TestFixture()]
	public class EventListenerTest {
		private EventListener<EventHandler> GetEmptyListener( TestEventPublisher publisher, EventHandler emptyHandler ) {
			return new EventListener<EventHandler>(
				h => publisher.EmptyEvent += h,
				h => publisher.EmptyEvent -= h,
				emptyHandler );
		}

		private EventListener<EventHandler<StringEventArgs>> GetStringListener( TestEventPublisher publisher, EventHandler<StringEventArgs> stringHandler ) {
			return new EventListener<EventHandler<StringEventArgs>>(
				h => publisher.StringEvent += h,
				h => publisher.StringEvent -= h,
				stringHandler );
		}

		[Test()]
		public void LifeCycleTest() {
			var listener1Success = false;
			var listener2Success = false;
			void Reset() {
				listener1Success = false;
				listener2Success = false;
			}

			var eventPublisher = new TestEventPublisher();

			var listener1 = GetEmptyListener( eventPublisher, ( sender, e ) => listener1Success = true );
			var listener2 = GetStringListener( eventPublisher, ( sender, e ) => listener2Success = true );

			//------------------
			Reset();

			eventPublisher.RaiseEmptyEvent();
			eventPublisher.RaiseStringEvent( string.Empty );

			listener1Success.Is( true );
			listener2Success.Is( true );

			//------------------
			Reset();

			listener1.Dispose();
			listener2.Dispose();
			eventPublisher.RaiseEmptyEvent();
			eventPublisher.RaiseStringEvent( string.Empty );

			listener1Success.Is( false );
			listener2Success.Is( false );
		}

	}
}
