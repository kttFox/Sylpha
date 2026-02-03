using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Sylpha.Commands {
	/// <summary>
	/// パラメーターを持つ汎用的なコマンドを表します。
	/// </summary>
	/// <typeparam name="T">コマンド パラメーターの型</typeparam>
	public sealed class DelegateCommand<T> : NotificationObject, ICommand, INotifyPropertyChanged {
		private readonly Action<T?> _execute;
		private readonly Func<T?, bool>? _canExecute;

		/// <summary>
		/// 指定されたコマンド実行メソッドと実行可能判定メソッドを使用して、<see cref="DelegateCommand{T}"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="execute">コマンド起動時に呼び出される <see cref="Action{T}"/> デリゲート</param>
		/// <param name="canExecute">コマンドが実行可能かどうかを判定する <see cref="Func{T,TResult}"/> デリゲート</param>
		public DelegateCommand( Action<T?> execute, Func<T?, bool>? canExecute = null ) {
			_execute = execute ?? throw new ArgumentNullException( nameof( execute ) );
			_canExecute = canExecute;
		}

		/// <summary>
		/// 現在のコマンドが実行可能かどうかを取得します。<br />
		/// このプロパティは <see cref="CanExecute(T?)"/> メソッドの呼び出しによって更新されます。
		/// </summary>
		public bool CurrentCanExecute { get; private set => SetProperty( ref field, value ); }

		/// <summary>
		/// コマンドが実行可能かどうかを取得します。
		/// </summary>
		/// <param name="parameter">コマンド パラメーター</param>
		/// <returns>コマンドが実行可能な場合は true、それ以外の場合は false</returns>
		public bool CanExecute( T? parameter ) => CurrentCanExecute = ( _canExecute?.Invoke( parameter ) ?? true );

		/// <summary>
		/// コマンドを実行します。
		/// </summary>
		/// <param name="parameter">コマンド パラメーター</param>
		public void Execute( T? parameter ) => _execute( parameter );

		/// <summary>
		/// コマンドが実行可能な場合のみ実行を試みます。
		/// </summary>
		/// <param name="parameter">コマンド パラメーター</param>
		public void TryExecute( T parameter ) {
			if( CanExecute( parameter ) ) {
				_execute( parameter );
			}
		}

		/// <inheritdoc />
		void ICommand.Execute( object? parameter ) => Execute( (T?)parameter );

		/// <inheritdoc />
		bool ICommand.CanExecute( object? parameter ) => CanExecute( (T?)parameter );

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