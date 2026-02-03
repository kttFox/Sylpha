using System.Windows;
using Sylpha.Messaging.Behaviors;

namespace Sylpha.Messaging {
	/// <summary>
	/// メッセージボックス用のメッセージ
	/// </summary>
	public class MessageBoxMessage : RequestMessage<MessageBoxResult> {
		#region TextProperty
		/// <summary>
		/// 表示するメッセージを指定、または取得します。
		/// </summary>
		public string Text {
			get => (string)GetValue( TextProperty );
			set => SetValue( TextProperty, value );
		}

		/// <summary>
		/// <see cref="Text"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register( nameof( Text ), typeof( string ), typeof( MessageBoxMessage ), new PropertyMetadata( "" ) );
		#endregion

		#region CaptionProperty
		/// <summary>
		/// キャプションを指定、または取得します。
		/// </summary>
		public string Caption {
			get => (string)GetValue( CaptionProperty );
			set => SetValue( CaptionProperty, value );
		}
		
		/// <summary>
		/// <see cref="Caption"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty CaptionProperty =
			DependencyProperty.Register( nameof( Caption ), typeof( string ), typeof( MessageBoxMessage ), new PropertyMetadata( "" ) );
		#endregion

		#region ImageProperty
		/// <summary>
		/// メッセージボックスイメージを指定、または取得します。
		/// </summary>
		public MessageBoxImage Image {
			get => (MessageBoxImage)GetValue( ImageProperty );
			set => SetValue( ImageProperty, value );
		}
		
		/// <summary>
		/// <see cref="Image"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty ImageProperty =
			DependencyProperty.Register( nameof( Image ), typeof( MessageBoxImage ), typeof( MessageBoxMessage ), new PropertyMetadata( MessageBoxImage.None ) );
		#endregion

		#region ButtonProperty
		/// <summary>
		/// メッセージボックスボタンを指定、または取得します。
		/// </summary>
		/// <returns>既定値は <see cref="MessageBoxButton.OK"/> です。</returns>
		public MessageBoxButton Button {
			get => (MessageBoxButton)GetValue( ButtonProperty );
			set => SetValue( ButtonProperty, value );
		}
		
		/// <summary>
		/// <see cref="Button"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty ButtonProperty =
			DependencyProperty.Register( nameof( Button ), typeof( MessageBoxButton ), typeof( MessageBoxMessage ), new PropertyMetadata( MessageBoxButton.OK ) );
		#endregion

		#region DefaultResultProperty
		/// <summary>
		/// メッセージボックスの既定の結果を指定、または取得します。
		/// </summary>
		/// <returns>既定値は <see cref="MessageBoxResult.None"/> です。</returns>
		public MessageBoxResult DefaultResult {
			get => (MessageBoxResult)GetValue( DefaultResultProperty );
			set => SetValue( DefaultResultProperty, value );
		}
		
		/// <summary>
		/// <see cref="DefaultResult"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty DefaultResultProperty =
			DependencyProperty.Register( nameof( DefaultResult ), typeof( MessageBoxResult ), typeof( MessageBoxMessage ), new PropertyMetadata( MessageBoxResult.None ) );
		#endregion

		#region Register IsOwned
		/// <summary>
		/// メッセージボックスにオーナーウィンドウを設定するかを指定、または取得します。
		/// </summary>
		public bool IsOwned {
			get => (bool)GetValue( IsOwnedProperty );
			set => SetValue( IsOwnedProperty, value );
		}
		
		/// <summary>
		/// <see cref="IsOwned"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty IsOwnedProperty =
			DependencyProperty.Register( nameof( IsOwned ), typeof( bool ), typeof( MessageBoxMessage ), new PropertyMetadata( true ) );
		#endregion

		/// <summary>
		/// <see cref="MessageBoxMessage"/> の新しいインスタンスを初期化します。
		/// </summary>
		public MessageBoxMessage() { }

		/// <summary>
		/// 表示するメッセージ、キャプション、メッセージボックスイメージ、メッセージボックスボタン、メッセージキーを指定して、<see cref="MessageBoxMessage"/> の新しいインスタンスを初期化します。
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

		/// <inheritdoc />
		protected override Freezable CreateInstanceCore() => new MessageBoxMessage();
	}

	/// <summary>
	/// <see cref="MessageBoxMessage"/> 用の拡張メソッドを提供します。
	/// </summary>
	public static class MessageBoxMessageExtensions {
		/// <summary>
		/// メッセージボックスメッセージを送信し、結果を取得します。
		/// </summary>
		/// <param name="messenger">送信先</param>
		/// <param name="message">送信するメッセージ</param>
		/// <returns>結果が設定されたメッセージ</returns>
		public static MessageBoxMessage MessageBox( this Messenger messenger, MessageBoxMessage message ) {
			return messenger.Raise( message );
		}
	}
}