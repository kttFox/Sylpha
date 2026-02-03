using System.Windows;

namespace Sylpha.Messaging {
	/// <summary>
	/// ファイルを保存する 用のメッセージ
	/// </summary>
	public class SaveFileDialogMessage : FileDialogMessage {
		#region CreatePromptProperty
		/// <summary>
		/// ユーザーが存在しないファイルを指定した場合に、ファイルを作成することを確認するメッセージを表示するかどうかを設定または取得します。既定値は <see langword="false"/> です。
		/// </summary>
		public bool CreatePrompt {
			get => (bool)GetValue( CreatePromptProperty );
			set => SetValue( CreatePromptProperty, value );
		}

		/// <summary>
		/// <see cref="CreatePrompt"/> 依存関係プロパティ
		/// </summary>
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

		/// <summary>
		/// <see cref="CreateTestFile"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty CreateTestFileProperty =
			DependencyProperty.Register( nameof( CreateTestFile ), typeof( bool ), typeof( SaveFileDialogMessage ), new PropertyMetadata( default( bool ) ) );
		#endregion
#endif

		#region OverwritePromptProperty
		/// <summary>
		/// ユーザーが指定したファイルが存在する場合、上書き確認メッセージを表示するかどうかを設定または取得します。既定値は <see langword="true"/> です。
		/// </summary>
		public bool OverwritePrompt {
			get => (bool)GetValue( OverwritePromptProperty );
			set => SetValue( OverwritePromptProperty, value );
		}

		/// <summary>
		/// <see cref="OverwritePrompt"/> 依存関係プロパティ
		/// </summary>
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
		/// 派生クラスでは必ずオーバーライドしてください。<see cref="Freezable"/> オブジェクトとして必要な実装です。<br />
		/// 通常、このメソッドは自身の新しいインスタンスを返すように実装します。
		/// </summary>
		protected override Freezable CreateInstanceCore() => new SaveFileDialogMessage();
	}

	/// <summary>
	/// <see cref="SaveFileDialogMessage"/> 用の拡張メソッドを提供します。
	/// </summary>
	public static class SaveFileDialogMessageExtensions {
		/// <summary>
		/// ファイル保存ダイアログを表示するメッセージを送信し、結果を取得します。
		/// </summary>
		/// <param name="messenger">送信先</param>
		/// <param name="message">送信するメッセージ</param>
		/// <returns>結果が設定されたメッセージ</returns>
		public static SaveFileDialogMessage SaveFileDialog( this Messenger messenger, SaveFileDialogMessage message ) {
			return messenger.Raise( message );
		}
	}
}