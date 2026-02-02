using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sylpha.Commands;
using System.Windows.Input;
using System.Threading;

namespace Sylpha.NUnit.Commands {
	[TestFixture()]
	public class CommandTest {

		[Test()]
		[Timeout( 3000 )]
		public void CanExecuteHandlerBasicTest() {
			var handlerResultList = new List<string>();

			var command = new DelegateCommand( () => { }, () => true );

			EventHandler handler1 = ( sender, e ) => handlerResultList.Add( "Handler1" );
			EventHandler handler2 = ( sender, e ) => handlerResultList.Add( "Handler2" );
			EventHandler handler3 = ( sender, e ) => handlerResultList.Add( "Handler3" );

			var semaphore = new Semaphore( 0, 1 );
			EventHandler releaseHandler = ( sender, e ) => semaphore.Release();

			command.CanExecuteChanged += handler1;
			command.CanExecuteChanged += handler2;
			command.CanExecuteChanged += handler3;
			command.CanExecuteChanged += releaseHandler;

			command.RaiseCanExecuteChanged();
			semaphore.WaitOne( 1000 ).Is( true );

			handlerResultList.Count.Is( 3 );
			handlerResultList.Is( "Handler1", "Handler2", "Handler3" );

			handlerResultList.Clear();

			command.CanExecuteChanged -= handler2;

			command.RaiseCanExecuteChanged();
			semaphore.WaitOne( 1000 ).Is( true );

			handlerResultList.Count.Is( 2 );
			handlerResultList.Is( "Handler1", "Handler3" );
		}

		[Test]
		public void CurrentCanExecuteTest() {
			var canExecute = false;
			var command = new DelegateCommand<int>( x => { }, x => canExecute );

			command.CurrentCanExecute.Is( false );

			canExecute = true;
			command.CanExecute( 0 );
			command.CurrentCanExecute.Is( true );

			canExecute = false;
			command.CanExecute( 0 );
			command.CurrentCanExecute.Is( false );
		}
	}
}