using System.Windows;

namespace Sylpha.Messaging {
	/// <summary>
	/// ファイルを保存する 用のメッセージです。
	/// </summary>
	public class SavingFileSelectionMessage : FileSelectionMessage {
		#region CreatePromptProperty
		/// <summary>
		/// ユーザーが存在しないファイルを指定した場合に、ファイルを作成することを確認するメッセージを表示するかどうかを指定、または取得します。デフォルトはfalseです。
		/// </summary>
		public bool CreatePrompt {
			get { return (bool)( GetValue( CreatePromptProperty ) ); }
			set { SetValue( CreatePromptProperty, value ); }
		}

		public static readonly DependencyProperty CreatePromptProperty =
			DependencyProperty.Register( nameof( CreatePrompt ), typeof( bool ), typeof( SavingFileSelectionMessage ), new PropertyMetadata( false ) );
		#endregion

		#region OverwritePromptProperty
		/// <summary>
		/// ユーザーが指定したファイルが存在する場合、上書き確認メッセージを表示するかどうかを指定、または取得します。デフォルトはtrueです。
		/// </summary>
		public bool OverwritePrompt {
			get { return (bool)( GetValue( OverwritePromptProperty ) ); }
			set { SetValue( OverwritePromptProperty, value ); }
		}

		public static readonly DependencyProperty OverwritePromptProperty =
			DependencyProperty.Register( nameof( OverwritePrompt ), typeof( bool ), typeof( SavingFileSelectionMessage ), new PropertyMetadata( true ) );
		#endregion

		public SavingFileSelectionMessage() { }

		/// <summary>
		/// メッセージキーを指定して新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public SavingFileSelectionMessage( string? messageKey ) : base( messageKey ) { }

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new SavingFileSelectionMessage( MessageKey );
		}
	}
}