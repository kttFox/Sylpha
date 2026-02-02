using System;
using System.Collections.Generic;
using System.Linq;

namespace Sylpha.Commands {
	public abstract class Command {
		private readonly List<WeakReference<EventHandler?>> _canExecuteChangedHandlers = [];

		/// <summary>
		/// コマンドが実行可能かどうかが変化した時に発生します。
		/// </summary>
		public event EventHandler? CanExecuteChanged {
			add => _canExecuteChangedHandlers.Add( new WeakReference<EventHandler?>( value ) );
			remove {
				var weakReferences = _canExecuteChangedHandlers.Where( r => r.TryGetTarget( out var result ) && result == value ).ToArray();
				foreach( var weakReference in weakReferences ) {
					_canExecuteChangedHandlers.Remove( weakReference );
				}
			}
		}

		/// <summary>
		/// コマンドが実行可能かどうかが変化したことを通知します。
		/// </summary>
		public virtual void RaiseCanExecuteChanged() {
			OnCanExecuteChanged();
		}

		/// <summary>
		/// コマンドが実行可能かどうかが変化した時に呼び出されます。
		/// </summary>
		protected void OnCanExecuteChanged() {
			var uiDispatcher = DispatcherHelper.UIDispatcher ?? throw new InvalidOperationException( "DispatcherHelper.UIDispatcher is null." );

			foreach( var weakReference in _canExecuteChangedHandlers.ToArray() ) {
				if( weakReference.TryGetTarget( out var result ) ) {
					uiDispatcher.InvokeAsync( () => result?.Invoke( this, EventArgs.Empty ) );
				} else {
					_canExecuteChangedHandlers.Remove( weakReference );
				}
			}
		}
	}
}