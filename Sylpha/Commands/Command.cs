using System;
using System.Collections.Generic;
using System.Linq;

namespace Sylpha.Commands {
	public abstract class Command {
		/// <summary>
		/// コマンドが実行可能かどうかが変化した時に発生します。
		/// </summary>
		public event EventHandler? CanExecuteChanged;

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
			CanExecuteChanged?.Invoke( this, EventArgs.Empty );
		}
	}
}