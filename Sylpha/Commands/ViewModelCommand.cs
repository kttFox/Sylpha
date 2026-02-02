using System;
using System.ComponentModel;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Sylpha.Commands {
	/// <summary>
	/// ViewModelがViewに公開するコマンドを表します。
	/// </summary>
	public sealed class ViewModelCommand : Command, ICommand, INotifyPropertyChanged {
		private readonly Func<bool>? _canExecute;
		private readonly Action _execute;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="execute">コマンドが実行するAction</param>
		/// <param name="canExecute">コマンドが実行可能かどうかをあらわすFunc&lt;bool&gt;</param>
		public ViewModelCommand( Action execute, Func<bool>? canExecute = null ) {
			_execute = execute ?? throw new ArgumentNullException( nameof( execute ) );
			_canExecute = canExecute;
		}

		/// <summary>
		/// コマンドが実行可能かどうかを取得します。
		/// </summary>
		public bool CanExecute => _canExecute?.Invoke() ?? true;

		void ICommand.Execute( object? parameter ) {
			Execute();
		}

		bool ICommand.CanExecute( object? parameter ) {
			return CanExecute;
		}

		/// <summary>
		/// コマンドが実行可能かどうかが変化した時に発生します。
		/// </summary>
		public event PropertyChangedEventHandler? PropertyChanged;

		/// <summary>
		/// コマンドを実行します。
		/// </summary>
		public void Execute() {
			_execute();
		}

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged() {
			PropertyChanged?.Invoke( this, EventArgsFactory.GetPropertyChangedEventArgs( nameof( CanExecute ) ) );
		}

		/// <summary>
		/// コマンドが実行可能かどうかが変化したことを通知します。
		/// </summary>
		public void RaiseCanExecuteChanged() {
			OnPropertyChanged();
			OnCanExecuteChanged();
		}
	}
}