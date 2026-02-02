using System.Windows;

namespace Sylpha.Messaging {
	/// <summary>
	/// 情報をViewに通知するためのメッセージです。
	/// </summary>
	public class InformationMessage : Message {
		#region TextProperty
		/// <summary>
		/// 表示するメッセージを指定、または取得します。
		/// </summary>
		public string? Text {
			get { return (string?)GetValue( TextProperty ); }
			set { SetValue( TextProperty, value ); }
		}

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register( nameof( Text ), typeof( string ), typeof( InformationMessage ), new PropertyMetadata( null ) );
		#endregion

		#region CaptionProperty
		/// <summary>
		/// キャプションを指定、または取得します。
		/// </summary>

		public string? Caption {
			get { return (string?)GetValue( CaptionProperty ); }
			set { SetValue( CaptionProperty, value ); }
		}

		public static readonly DependencyProperty CaptionProperty =
			DependencyProperty.Register( nameof( Caption ), typeof( string ), typeof( InformationMessage ), new PropertyMetadata( null ) );
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
			DependencyProperty.Register( nameof( Image ), typeof( MessageBoxImage ), typeof( InformationMessage ), new PropertyMetadata( MessageBoxImage.None ) );
		#endregion

		public InformationMessage() { }

		/// <summary>
		/// 表示するメッセージ・キャプション・メッセージボックスイメージ・メッセージキーを指定して、新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="text">表示するメッセージ</param>
		/// <param name="caption">キャプション</param>
		/// <param name="image">メッセージボックスイメージ</param>
		/// <param name="messageKey">メッセージキー</param>
		public InformationMessage( string text, string caption, MessageBoxImage image, string? messageKey ) : base( messageKey ) {
			Text = text;
			Caption = caption;
			MessageKey = messageKey;
			Image = image;
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new InformationMessage();
		}
	}
}