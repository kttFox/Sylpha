using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using JetBrains.Annotations;

namespace Sylpha.Messaging {
	/// <summary>
	/// 画面遷移アクション用のメッセージです。
	/// </summary>
	[ContentProperty( nameof( ViewModel ) )]
	[PublicAPI]
	public class ShowWindowMessage : RequestMessage<bool?> {
		#region Register ViewModel
		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelを指定、または取得します。
		/// </summary>
		public INotifyPropertyChanged? ViewModel {
			get { return (INotifyPropertyChanged?)GetValue( ViewModelProperty ); }
			set { SetValue( ViewModelProperty, value ); }
		}

		public static readonly DependencyProperty ViewModelProperty =
					DependencyProperty.Register( nameof( ViewModel ), typeof( INotifyPropertyChanged ), typeof( ShowWindowMessage ), new PropertyMetadata( null ) );
		#endregion

		#region Register Mode
		/// <summary>
		/// 新しいWindowの表示方法を決定する<see cref="ShowWindowMode"/>を指定、または取得します。<br />
		/// 初期値は <see cref="ShowWindowMode.Modal"/> です。
		/// </summary>
		public ShowWindowMode Mode {
			get { return (ShowWindowMode)( GetValue( ModeProperty ) ); }
			set { SetValue( ModeProperty, value ); }
		}
		public static readonly DependencyProperty ModeProperty =
									DependencyProperty.Register( nameof( Mode ), typeof( ShowWindowMode? ), typeof( ShowWindowMessage ), new PropertyMetadata( ShowWindowMode.Modal ) );
		#endregion

		#region Register WindowType
		/// <summary>
		/// 新しいWindowの型を指定、または取得します。
		/// </summary>
		public Type? WindowType {
			get { return (Type?)GetValue( WindowTypeProperty ); }
			set { SetValue( WindowTypeProperty, value ); }
		}

		public static readonly DependencyProperty WindowTypeProperty =
					DependencyProperty.Register( nameof( WindowType ), typeof( Type ), typeof( ShowWindowMessage ), new PropertyMetadata( null ) );
		#endregion

		#region Register IsOwned
		/// <summary>
		/// 遷移先ウィンドウがアクションのウィンドウに所有されるかを設定します。
		/// </summary>
		public bool IsOwned {
			get { return (bool)GetValue( IsOwnedProperty ); }
			set { SetValue( IsOwnedProperty, value ); }
		}

		public static readonly DependencyProperty IsOwnedProperty =
			DependencyProperty.Register( nameof( IsOwned ), typeof( bool ), typeof( ShowWindowMessage ), new PropertyMetadata( true ) );
		#endregion

		#region Register WindowSettingAction
		/// <summary>
		/// ウインドウの設定を行う関数を設定または取得します。
		/// </summary>
		public Action<Window>? WindowSettingAction {
			get => (Action<Window>?)GetValue( WindowSettingActionProperty );
			set => SetValue( WindowSettingActionProperty, value );
		}

		public static readonly DependencyProperty WindowSettingActionProperty =
			DependencyProperty.Register( nameof( WindowSettingAction ), typeof( Action<Window> ), typeof( ShowWindowMessage ), new PropertyMetadata( null ) );
		#endregion

		#region Register InitializeAction
		/// <summary>
		/// ウインドウコンテンツがレンダリングされた後に実行する関数を設定または取得します。
		/// </summary>
		public Action<Window>? InitializeAction {
			get => (Action<Window>?)GetValue( InitializeActionProperty );
			set => SetValue( InitializeActionProperty, value );
		}

		public static readonly DependencyProperty InitializeActionProperty =
			DependencyProperty.Register( nameof( InitializeAction ), typeof( Action<Window> ), typeof( ShowWindowMessage ), new PropertyMetadata( null ) );
		#endregion


		/// <summary>
		/// メッセージのインスタンスを生成します。
		/// </summary>
		public ShowWindowMessage() { }

		/// <summary>
		/// 新しいWindowの型と新しいWindowに設定するViewModel、画面遷移モードとメッセージキーを指定して新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="windowType">新しいWindowの型</param>
		/// <param name="viewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="messageKey">メッセージキー</param>
		public ShowWindowMessage( Type? windowType = null, INotifyPropertyChanged? viewModel = null, string? messageKey = null ) : base( messageKey ) {
			this.ViewModel = viewModel;

			if( windowType != null ) {
				if( !windowType.IsSubclassOf( typeof( Window ) ) )
					throw new ArgumentException( "Windowの派生クラスを指定してください。", nameof( windowType ) );
			}

			WindowType = windowType;
		}


		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		protected override Freezable CreateInstanceCore() => new ShowWindowMessage();
	}
}