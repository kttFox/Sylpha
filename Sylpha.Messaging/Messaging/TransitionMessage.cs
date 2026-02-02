using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using JetBrains.Annotations;

namespace Sylpha.Messaging {
	/// <summary>
	/// 画面遷移アクション用のメッセージです。
	/// </summary>
	[ContentProperty( nameof( TransitionViewModel ) )]
	[PublicAPI]
	public class TransitionMessage : RequestMessage<bool?> {
		#region Register TransitionViewModelProperty
		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelを指定、または取得します。
		/// </summary>
		public INotifyPropertyChanged? TransitionViewModel {
			get { return (INotifyPropertyChanged?)GetValue( TransitionViewModelProperty ); }
			set { SetValue( TransitionViewModelProperty, value ); }
		}

		public static readonly DependencyProperty TransitionViewModelProperty =
					DependencyProperty.Register( nameof( TransitionViewModel ), typeof( INotifyPropertyChanged ), typeof( TransitionMessage ), new PropertyMetadata( null ) );
		#endregion

		#region Register ModeProperty
		/// <summary>
		/// 新しいWindowの表示方法を決定するTransitionModeを指定、または取得します。<br />
		/// 初期値はUnKnownです。
		/// </summary>
		public TransitionMode Mode {
			get { return (TransitionMode)( GetValue( ModeProperty ) ); }
			set { SetValue( ModeProperty, value ); }
		}
		public static readonly DependencyProperty ModeProperty =
									DependencyProperty.Register( nameof( Mode ), typeof( TransitionMode ), typeof( TransitionMessage ), new PropertyMetadata( TransitionMode.UnKnown ) );
		#endregion

		#region Register WindowTypeProperty
		/// <summary>
		/// 新しいWindowの型を指定、または取得します。
		/// </summary>
		public Type? WindowType {
			get { return (Type?)GetValue( WindowTypeProperty ); }
			set { SetValue( WindowTypeProperty, value ); }
		}

		public static readonly DependencyProperty WindowTypeProperty =
					DependencyProperty.Register( nameof( WindowType ), typeof( Type ), typeof( TransitionMessage ), new PropertyMetadata( null ) );
		#endregion

		/// <summary>
		/// メッセージキーを指定して新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public TransitionMessage( string? messageKey ) : base( messageKey ) { }

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelとメッセージキーを指定して新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="transitionViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="messageKey">メッセージキー</param>
		public TransitionMessage( INotifyPropertyChanged transitionViewModel, string? messageKey ) : this( null, transitionViewModel, TransitionMode.UnKnown, messageKey ) { }

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelと画面遷移モードとメッセージキーを指定して新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="transitionViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。初期値はUnKnownです。</param>
		/// <param name="messageKey">メッセージキー</param>
		public TransitionMessage( INotifyPropertyChanged transitionViewModel, TransitionMode mode, string? messageKey ) : this( null, transitionViewModel, mode, messageKey ) { }

		/// <summary>
		/// 新しいWindowの型と新しいWindowに設定するViewModel、画面遷移モードとメッセージキーを指定して新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="windowType">新しいWindowの型</param>
		/// <param name="transitionViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。初期値はUnKnownです。</param>
		/// <param name="messageKey">メッセージキー</param>
		public TransitionMessage( Type? windowType, INotifyPropertyChanged transitionViewModel, TransitionMode mode, string? messageKey = null ) : base( messageKey ) {
			Mode = mode;
			TransitionViewModel = transitionViewModel;

			if( windowType != null ) {
				if( !windowType.IsSubclassOf( typeof( Window ) ) )
					throw new ArgumentException( "Windowの派生クラスを指定してください。", nameof( windowType ) );
			}

			WindowType = windowType;
		}

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelを指定して新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="transitionViewModel">新しいWindowのDataContextに設定するViewModel</param>
		public TransitionMessage( INotifyPropertyChanged transitionViewModel ) : this( null, transitionViewModel, TransitionMode.UnKnown ) { }

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelと画面遷移モードを指定して新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="transitionViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。初期値はUnKnownです。</param>
		public TransitionMessage( INotifyPropertyChanged transitionViewModel, TransitionMode mode ) : this( null, transitionViewModel, mode ) { }


		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new TransitionMessage( TransitionViewModel, MessageKey );
		}
	}
}