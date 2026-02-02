using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Sylpha.Commands {
	/// <summary>
	/// 汎用的コマンドを表します。
	/// </summary>
	public sealed class DelegateCommand : NotificationObject, ICommand, INotifyPropertyChanged {
		private readonly Action _execute;
		private readonly Func<bool>? _canExecute;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="execute">コマンドが実行するAction</param>
		/// <param name="canExecute">コマンドが実行可能かどうかをあらわすFunc&lt;bool&gt;</param>
		public DelegateCommand( Action execute, Func<bool>? canExecute = null ) {
			_execute = execute ?? throw new ArgumentNullException( nameof( execute ) );
			_canExecute = canExecute;
		}

		/// <summary>
		/// 現在のコマンドが実行可能かどうかを取得します。<br />CanExecute()の呼び出しによって更新されます。
		/// </summary>
		public bool CurrentCanExecute { get; private set => SetProperty( ref field, value ); }

		/// <summary>
		/// コマンドが実行可能かどうかを取得します。
		/// </summary>
		public bool CanExecute() => CurrentCanExecute = ( _canExecute?.Invoke() ?? true );

		/// <summary>
		/// コマンドを実行します。
		/// </summary>
		public void Execute() => _execute();

		/// <summary>
		/// コマンドを試行します。
		/// </summary>
		public void TryExecute() {
			if( CanExecute() ) {
				_execute();
			}
		}

		void ICommand.Execute( object? parameter ) => Execute();

		bool ICommand.CanExecute( object? parameter ) => CanExecute();

		/// <summary>
		/// コマンドが実行可能かどうかが変化した時に発生します。
		/// </summary>
		public event EventHandler? CanExecuteChanged;

		/// <summary>
		/// コマンドが実行可能かどうかが変化したことを通知します。
		/// </summary>
		public void RaiseCanExecuteChanged() {
			CanExecuteChanged?.Invoke( this, EventArgs.Empty );
		}
	}
}