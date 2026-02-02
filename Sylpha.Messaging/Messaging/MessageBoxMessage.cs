using System.Windows;
using Sylpha.Messaging.Behaviors;

namespace Sylpha.Messaging {
	/// <summary>
	/// 確認メッセージを表します。
	/// </summary>
	public class MessageBoxMessage : RequestMessage<MessageBoxResult> {

		#region TextProperty
		/// 表示するメッセージを指定、または取得します。
		/// </summary>
		public string Text {
			get { return (string)GetValue( TextProperty ); }
			set { SetValue( TextProperty, value ); }
		}

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register( nameof( Text ), typeof( string ), typeof( MessageBoxMessage ), new PropertyMetadata( "" ) );
		#endregion

		#region CaptionProperty
		/// <summary>
		/// キャプションを指定、または取得します。
		/// </summary>
		public string Caption {
			get { return (string)GetValue( CaptionProperty ); }
			set { SetValue( CaptionProperty, value ); }
		}
		public static readonly DependencyProperty CaptionProperty =
			DependencyProperty.Register( nameof( Caption ), typeof( string ), typeof( MessageBoxMessage ), new PropertyMetadata( "" ) );
		#endregion

		#region ImageProperty
		/// <summary>
		/// メッセージボックスイメージを指定、または取得します。
		/// </summary>
		public MessageBoxImage Image {
			get { return (MessageBoxImage)( GetValue( ImageProperty ) ); }
			set { SetValue( ImageProperty, value ); }
		}

		public static readonly DependencyProperty ImageProperty =
			DependencyProperty.Register( nameof( Image ), typeof( MessageBoxImage ), typeof( MessageBoxMessage ), new PropertyMetadata( MessageBoxImage.None ) );
		#endregion

		#region ButtonProperty
		/// <summary>
		/// メッセージボックスボタンを指定、または取得します。
		/// </summary>
		/// <returns>既定値は <see cref="MessageBoxButton.OK"/> です。</returns>
		public MessageBoxButton Button {
			get { return (MessageBoxButton)( GetValue( ButtonProperty ) ); }
			set { SetValue( ButtonProperty, value ); }
		}

		public static readonly DependencyProperty ButtonProperty =
			DependencyProperty.Register( nameof( Button ), typeof( MessageBoxButton ), typeof( MessageBoxMessage ), new PropertyMetadata( MessageBoxButton.OK ) );
		#endregion

		#region DefaultResultProperty
		/// <summary>
		/// メッセージボックスの既定の結果を指定、または取得します。
		/// </summary>
		/// <returns>既定値は <see cref="MessageBoxResult.None"/> です。</returns>
		public MessageBoxResult DefaultResult {
			get { return (MessageBoxResult)( GetValue( DefaultResultProperty ) ); }
			set { SetValue( DefaultResultProperty, value ); }
		}

		public static readonly DependencyProperty DefaultResultProperty =
			DependencyProperty.Register( nameof( DefaultResult ), typeof( MessageBoxResult ), typeof( MessageBoxMessage ), new PropertyMetadata( MessageBoxResult.None ) );
		#endregion

		#region Register IsOwned
		/// <summary>
		/// メッセージボックスにオーナーウィンドウを設定するかを指定、または取得します。
		/// </summary>
		public bool IsOwned {
			get { return (bool)( GetValue( IsOwnedProperty ) ); }
			set { SetValue( IsOwnedProperty, value ); }
		}

		public static readonly DependencyProperty IsOwnedProperty =
			DependencyProperty.Register( nameof( IsOwned ), typeof( bool ), typeof( MessageBoxMessage ), new PropertyMetadata( true ) );
		#endregion

		/// <summary>
		/// 新しいメッセージのインスタンスを生成します。
		/// </summary>
		public MessageBoxMessage() { }

		/// <summary>
		/// 表示するメッセージ・キャプション・メッセージボックスイメージ・メッセージボックスボタン・メッセージキーを指定して、新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="text">表示するメッセージ</param>
		/// <param name="caption">キャプション</param>
		/// <param name="image">メッセージボックスイメージ</param>
		/// <param name="button">メッセージボックスボタン</param>
		/// <param name="messageKey">メッセージキー</param>
		public MessageBoxMessage( string text, string caption = "", MessageBoxImage image = MessageBoxImage.None, MessageBoxButton button = MessageBoxButton.OK, string? messageKey = null ) : base( messageKey ) {
			Text = text;
			Caption = caption;
			Image = image;
			Button = button;
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		protected override Freezable CreateInstanceCore() => new MessageBoxMessage();
	}
}