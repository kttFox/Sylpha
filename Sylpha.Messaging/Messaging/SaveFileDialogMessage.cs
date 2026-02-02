using System.Windows;

namespace Sylpha.Messaging {
	/// <summary>
	/// ファイルを保存する 用のメッセージです。
	/// </summary>
	public class SaveFileDialogMessage : FileDialogMessage {
		#region CreatePromptProperty
		/// <summary>
		/// ユーザーが存在しないファイルを指定した場合に、ファイルを作成することを確認するメッセージを表示するかどうかを設定または取得します。既定値は <see langword="false"/> です。
		/// </summary>
		public bool CreatePrompt {
			get { return (bool)( GetValue( CreatePromptProperty ) ); }
			set { SetValue( CreatePromptProperty, value ); }
		}

		public static readonly DependencyProperty CreatePromptProperty =
			DependencyProperty.Register( nameof( CreatePrompt ), typeof( bool ), typeof( SaveFileDialogMessage ), new PropertyMetadata( false ) );
		#endregion
#if NET8_0_OR_GREATER

		#region Register CreateTestFile
		/// <summary>
		/// ダイアログ ボックスが選択したパスでテスト ファイルの作成を試みるかどうかを示す値を設定または取得します。
		/// </summary>
		/// <returns>テスト ファイルの作成を試みる場合は <see langword="true"/>。それ以外の場合は <see langword="false"/>。 既定値は <see langword="true"/> です。</returns>
		public bool CreateTestFile {
			get => (bool)GetValue( CreateTestFileProperty );
			set => SetValue( CreateTestFileProperty, value );
		}

		public static readonly DependencyProperty CreateTestFileProperty =
			DependencyProperty.Register( nameof( CreateTestFile ), typeof( bool ), typeof( SaveFileDialogMessage ), new PropertyMetadata( default( bool ) ) );
		#endregion
#endif

		#region OverwritePromptProperty
		/// <summary>
		/// ユーザーが指定したファイルが存在する場合、上書き確認メッセージを表示するかどうかを設定または取得します。既定値は <see langword="true"/> です。
		/// </summary>
		public bool OverwritePrompt {
			get { return (bool)( GetValue( OverwritePromptProperty ) ); }
			set { SetValue( OverwritePromptProperty, value ); }
		}

		public static readonly DependencyProperty OverwritePromptProperty =
			DependencyProperty.Register( nameof( OverwritePrompt ), typeof( bool ), typeof( SaveFileDialogMessage ), new PropertyMetadata( true ) );
		#endregion

		/// <summary>
		/// 新しいメッセージのインスタンスを生成します。
		/// </summary>
		public SaveFileDialogMessage() { }

		/// <summary>
		/// メッセージキーを指定して新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public SaveFileDialogMessage( string? messageKey ) : base( messageKey ) { }

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() => new SaveFileDialogMessage();
	}
}