using System;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Threading;

namespace Sylpha.NUnit {
	public class TestDispatcherContext : IDisposable {
		private Dispatcher? _dispatcher = null;

		public TestDispatcherContext() {
			var dispatcherRunFlag = false;

			Task.Factory.StartNew( () => {
				_dispatcher = Dispatcher.CurrentDispatcher;
				Dispatcher.Run();
			}, TaskCreationOptions.LongRunning );

			while( _dispatcher == null ) { Thread.Sleep( 10 ); }

			_dispatcher.BeginInvoke( (Action)( () => dispatcherRunFlag = true ) );

			while( !dispatcherRunFlag ) { Thread.Sleep( 10 ); }
		}


		public Dispatcher? Dispatcher {
			get {
				ThrowExceptionIfDisposed();
				return _dispatcher;
			}
		}

		protected void ThrowExceptionIfDisposed() {
			if( _disposed ) {
				throw new ObjectDisposedException( "DispatcherContext" );
			}
		}

		#region dispose
		public void Dispose() {
			Dispose( true );
		}

		private bool _disposed = false;

		protected virtual void Dispose( bool disposing ) {
			if( _disposed ) return;

			if( disposing ) {
				if( _dispatcher != null && !( _dispatcher.HasShutdownStarted || _dispatcher.HasShutdownFinished ) ) {
					_dispatcher.InvokeShutdown();
				}
			}
			_disposed = true;
		}
		#endregion
	}
}
