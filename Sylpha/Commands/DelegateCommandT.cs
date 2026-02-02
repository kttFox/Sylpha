using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Sylpha.Commands {
	/// <summary>
	/// <see cref="{T}"/>型オブジェクトを受け取る汎用的コマンドを表します。
	/// </summary>
	/// <typeparam name="T">受け取るオブジェクトの型</typeparam>
	public sealed class DelegateCommand<T> : NotificationObject, ICommand, INotifyPropertyChanged {
		private readonly Action<T?> _execute;
		private readonly Func<T?, bool>? _canExecute;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="execute">コマンドが実行するAction</param>
		/// <param name="canExecute">コマンドが実行可能かどうかをあらわすFunc&lt;bool&gt;</param>
		public DelegateCommand( Action<T?> execute, Func<T?, bool>? canExecute = null ) {
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
		public bool CanExecute( T? parameter ) => CurrentCanExecute = ( _canExecute?.Invoke( parameter ) ?? true );

		/// <summary>
		/// コマンドを実行します。
		/// </summary>
		/// <param name="parameter">Viewから渡されたオブジェクト</param>
		public void Execute( T? parameter ) => _execute( parameter );

		/// <summary>
		/// コマンドを試行します。
		/// </summary>
		public void TryExecute( T parameter ) {
			if( CanExecute( parameter ) ) {
				_execute( parameter );
			}
		}

		void ICommand.Execute( object? parameter ) => Execute( (T?)parameter );

		bool ICommand.CanExecute( object? parameter ) => CanExecute( (T?)parameter );
		
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