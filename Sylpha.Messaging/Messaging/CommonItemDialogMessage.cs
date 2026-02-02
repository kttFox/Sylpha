using System.Windows;

namespace Sylpha.Messaging {
	/// <summary>
	/// アイテムダイアログ（ファイル、フォルダー選択など）用の共通メッセージ基底抽象クラスです。
	/// </summary>
	public abstract class CommonItemDialogMessage : RequestMessage<string[]> {
		#region Register InitialDirectory
		/// <summary>
		/// 初期ディレクトリを設定または取得します。
		/// </summary>
		public string InitialDirectory {
			get { return (string)GetValue( InitialDirectoryProperty ); }
			set { SetValue( InitialDirectoryProperty, value ); }
		}

		public static readonly DependencyProperty InitialDirectoryProperty =
			DependencyProperty.Register( nameof( InitialDirectory ), typeof( string ), typeof( CommonItemDialogMessage ), new PropertyMetadata( "" ) );
		#endregion

		#region Register RestoreDirectoryGroup
		/// <summary>
		/// 以前に選択したディレクトリ情報が共有されるグループを設定または取得します。<br/>
		/// このプロパティが有効な場合、InitialDirectoryプロパティより優先されます。
		/// </summary>
		public string RestoreDirectoryGroup {
			get { return (string)GetValue( RestoreDirectoryGroupProperty ); }
			set { SetValue( RestoreDirectoryGroupProperty, value ); }
		}

		public static readonly DependencyProperty RestoreDirectoryGroupProperty =
			DependencyProperty.Register( nameof( RestoreDirectoryGroup ), typeof( string ), typeof( CommonItemDialogMessage ), new PropertyMetadata( "" ) );
		#endregion


		#region Register Title
		/// <summary>
		/// タイトルを設定または取得します。
		/// </summary>
		public string Title {
			get { return (string)GetValue( TitleProperty ); }
			set { SetValue( TitleProperty, value ); }
		}

		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register( nameof( Title ), typeof( string ), typeof( CommonItemDialogMessage ), new PropertyMetadata( "" ) );
		#endregion

		/// <summary>
		/// 新しいメッセージのインスタンスを生成します。
		/// </summary>
		protected CommonItemDialogMessage() { }

		/// <summary>
		/// メッセージキーを指定して、新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		protected CommonItemDialogMessage( string? messageKey ) : base( messageKey ) { }

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected abstract override Freezable CreateInstanceCore();
	}
}