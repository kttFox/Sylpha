using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Sylpha.Commands {
	/// <summary>
	/// 汎用的なコマンドを表します。
	/// </summary>
	public sealed class DelegateCommand : NotificationObject, ICommand, INotifyPropertyChanged {
		private readonly Action _execute;
		private readonly Func<bool>? _canExecute;

		/// <summary>
		/// 指定されたコマンド実行メソッドと実行可能判定メソッドを使用して、<see cref="DelegateCommand"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="execute">コマンド起動時に呼び出される <see cref="Action"/> デリゲート</param>
		/// <param name="canExecute">コマンドが実行可能かどうかを判定する <see cref="Func{TResult}"/> デリゲート</param>
		public DelegateCommand( Action execute, Func<bool>? canExecute = null ) {
			_execute = execute ?? throw new ArgumentNullException( nameof( execute ) );
			_canExecute = canExecute;
		}

		/// <summary>
		/// 現在のコマンドが実行可能かどうかを取得します。<br />
		/// このプロパティは <see cref="CanExecute"/> メソッドの呼び出しによって更新されます。
		/// </summary>
		public bool CurrentCanExecute { get; private set => SetProperty( ref field, value ); }

		/// <summary>
		/// コマンドが実行可能かどうかを取得します。
		/// </summary>
		/// <returns>コマンドが実行可能な場合は true、それ以外の場合は false</returns>
		public bool CanExecute() => CurrentCanExecute = ( _canExecute?.Invoke() ?? true );

		/// <summary>
		/// コマンドを実行します。
		/// </summary>
		public void Execute() => _execute();

		/// <summary>
		/// コマンドが実行可能な場合のみ実行を試みます。
		/// </summary>
		public void TryExecute() {
			if( CanExecute() ) {
				_execute();
			}
		}

		/// <inheritdoc />
		void ICommand.Execute( object? parameter ) => Execute();

		/// <inheritdoc />
		bool ICommand.CanExecute( object? parameter ) => CanExecute();

		/// <summary>
		/// コマンドの実行可能状態が変化したときに発生します。
		/// </summary>
		public event EventHandler? CanExecuteChanged;

		/// <summary>
		/// <see cref="CanExecuteChanged"/> イベントを発生させます。
		/// </summary>
		public void RaiseCanExecuteChanged() {
			CanExecuteChanged?.Invoke( this, EventArgs.Empty );
		}
	}
}