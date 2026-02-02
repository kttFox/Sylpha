using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace Sylpha.Messaging {
	/// <summary>
	/// 画面遷移アクション用のメッセージです。
	/// </summary>
	[ContentProperty( nameof( ViewModel ) )]
	public class ShowWindowMessage : RequestMessage<bool?> {
		#region Register ViewModel
		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelを設定または取得します。
		/// </summary>
		public INotifyPropertyChanged? ViewModel {
			get => (INotifyPropertyChanged?)GetValue( ViewModelProperty );
			set => SetValue( ViewModelProperty, value );
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
			get => (ShowWindowMode)( GetValue( ModeProperty ) );
			set => SetValue( ModeProperty, value );
		}
		public static readonly DependencyProperty ModeProperty =
									DependencyProperty.Register( nameof( Mode ), typeof( ShowWindowMode? ), typeof( ShowWindowMessage ), new PropertyMetadata( ShowWindowMode.Modal ) );
		#endregion

		#region Register WindowType
		/// <summary>
		/// 新しいWindowの型を指定、または取得します。
		/// </summary>
		public Type? WindowType {
			get => (Type?)GetValue( WindowTypeProperty );
			set => SetValue( WindowTypeProperty, value );
		}

		public static readonly DependencyProperty WindowTypeProperty =
					DependencyProperty.Register( nameof( WindowType ), typeof( Type ), typeof( ShowWindowMessage ), new PropertyMetadata( null ) );
		#endregion

		#region Register IsOwned
		/// <summary>
		/// 遷移先ウィンドウがアクションのウィンドウに所有されるかを設定します。
		/// </summary>
		public bool IsOwned {
			get => (bool)GetValue( IsOwnedProperty );
			set => SetValue( IsOwnedProperty, value );
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

			this.WindowType = windowType;
		}

		/// <summary>
		/// 表示したWindowのViewModelを取得します。
		/// </summary>
		public INotifyPropertyChanged? WindowViewModel { get; internal set; }

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		protected override Freezable CreateInstanceCore() => new ShowWindowMessage();
	}

	/// <summary>
	/// 画面遷移アクション用のメッセージです。
	/// </summary>
	/// <typeparam name="TWindow">ウインドウタイプ</typeparam>
	[ContentProperty( nameof( ViewModel ) )]
	public class ShowWindowMessage<TWindow> : ShowWindowMessage where TWindow : Window {
		public ShowWindowMessage() : base( typeof( TWindow ) ) { }

		public ShowWindowMessage( INotifyPropertyChanged? viewModel = null, string? messageKey = null ) : base( typeof( TWindow ), viewModel, messageKey ) {

		}

		/// <summary>
		/// ウインドウの設定を行う関数
		/// </summary>
		public new Action<TWindow>? WindowSettingAction {
			get => base.WindowSettingAction;
			set => base.WindowSettingAction = window => value?.Invoke( (TWindow)window );
		}

		/// <summary>
		/// ウインドウコンテンツがレンダリングされた後に実行する関数
		/// </summary>
		public new Action<TWindow>? InitializeAction {
			get => base.InitializeAction;
			set => base.InitializeAction = window => value?.Invoke( (TWindow)window );
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		protected override Freezable CreateInstanceCore() => new ShowWindowMessage<TWindow>();
	}

	/// <summary>
	/// 画面遷移アクション用のメッセージです。
	/// </summary>
	/// <typeparam name="TWindow">ウインドウタイプ</typeparam>
	/// <typeparam name="TViewModel">ViewModelのタイプ</typeparam>
	[ContentProperty( nameof( ViewModel ) )]
	public class ShowWindowMessage<TWindow, TViewModel> : ShowWindowMessage<TWindow> where TWindow : Window where TViewModel : INotifyPropertyChanged {
		public ShowWindowMessage() { }

		public ShowWindowMessage( TViewModel viewModel, string? messageKey = null ) : base( viewModel, messageKey ) {

		}

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelを設定または取得します。
		/// </summary>
		public new TViewModel? ViewModel {
			get => (TViewModel?)base.ViewModel;
			set => base.ViewModel = value;
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		protected override Freezable CreateInstanceCore() => new ShowWindowMessage<TWindow, TViewModel>();
	}
	public static class ShowWindowMessageGenerator<TWindow> where TWindow : Window {
		public static ShowWindowMessage<TWindow, TViewModel> Create<TViewModel>( TViewModel viewModel, string? messageKey = null ) where TViewModel : INotifyPropertyChanged {
			return new ShowWindowMessage<TWindow, TViewModel>( viewModel, messageKey );
		}
	}

	public static class ShowWindowExtensions {
		/// <summary>
		/// ウインドウ表示メッセージを送信し、ウインドウを表示します。
		/// </summary>
		/// <param name="messenger">送信先</param>
		/// <param name="message">送信するメッセージ</param>
		/// <returns>送信されたウインドウ表示メッセージ</returns>
		public static ShowWindowMessage ShowWindow( this Messenger messenger, ShowWindowMessage message ) {
			return messenger.Raise( message );
		}

		/// <summary>
		/// ウインドウ表示メッセージを送信し、指定された型のウインドウを表示します。
		/// </summary>
		/// <typeparam name="TWindow">表示するウインドウの型</typeparam>
		/// <param name="messenger">送信先</param>
		/// <param name="message">送信するメッセージ</param>
		/// <returns>結果が設定されたメッセージ</returns>
		public static ShowWindowMessage<TWindow> ShowWindow<TWindow>( this Messenger messenger, ShowWindowMessage<TWindow> message ) where TWindow : Window {
			return messenger.Raise( message );
		}

		/// <summary>
		/// ウインドウ表示メッセージを送信し、指定された型のウインドウとViewModelでウインドウを表示します。
		/// </summary>
		/// <typeparam name="TWindow">表示するウインドウの型</typeparam>
		/// <typeparam name="TViewModel">ウインドウに設定するViewModelの型</typeparam>
		/// <param name="messenger">送信先</param>
		/// <param name="message">送信するメッセージ</param>
		/// <returns>結果が設定されたメッセージ</returns>
		public static ShowWindowMessage<TWindow, TViewModel> ShowWindow<TWindow, TViewModel>( this Messenger messenger, ShowWindowMessage<TWindow, TViewModel> message ) where TWindow : Window where TViewModel : INotifyPropertyChanged {
			return messenger.Raise( message );
		}
	}
}