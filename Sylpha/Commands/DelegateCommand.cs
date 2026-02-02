using System;
using System.ComponentModel;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Sylpha.Commands {
	/// <summary>
	/// 汎用的コマンドを表します。
	/// </summary>
	public sealed class DelegateCommand : Command, ICommand, INotifyPropertyChanged {
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
		/// コマンドが実行可能かどうかを取得します。
		/// </summary>
		public bool CanExecute() => CurrentCanExecute = ( _canExecute?.Invoke() ?? true );

		/// <summary>
		/// 現在のコマンドが実行可能かどうかを取得します。
		/// </summary>
		public bool CurrentCanExecute {
			get;
			private set {
				if( field != value ) {
					field = value;
					OnPropertyChanged();
				}
			}
		}

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
		public event PropertyChangedEventHandler? PropertyChanged;


		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged() {
			PropertyChanged?.Invoke( this, EventArgsFactory.GetPropertyChangedEventArgs( nameof( CurrentCanExecute ) ) );
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