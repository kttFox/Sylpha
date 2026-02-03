using System.Windows;

namespace Sylpha.Messaging {
	/// <summary>
	/// アイテムダイアログ（ファイル、フォルダー選択など）用の共通メッセージ基底抽象クラス
	/// </summary>
	public abstract class CommonItemDialogMessage : RequestMessage<string[]> {
		#region Register InitialDirectory
		/// <summary>
		/// 初期ディレクトリを設定または取得します。
		/// </summary>
		public string InitialDirectory {
			get => (string)GetValue( InitialDirectoryProperty );
			set => SetValue( InitialDirectoryProperty, value );
		}
		
		/// <summary>
		/// <see cref="InitialDirectory"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty InitialDirectoryProperty =
			DependencyProperty.Register( nameof( InitialDirectory ), typeof( string ), typeof( CommonItemDialogMessage ), new PropertyMetadata( "" ) );
		#endregion

		#region Register RestoreDirectoryGroup
		/// <summary>
		/// 以前に選択したディレクトリ情報が共有されるグループを設定または取得します。<br />
		/// このプロパティが有効な場合、InitialDirectoryプロパティより優先されます。
		/// </summary>
		public string RestoreDirectoryGroup {
			get => (string)GetValue( RestoreDirectoryGroupProperty );
			set => SetValue( RestoreDirectoryGroupProperty, value );
		}
		
		/// <summary>
		/// <see cref="RestoreDirectoryGroup"/> 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty RestoreDirectoryGroupProperty =
			DependencyProperty.Register( nameof( RestoreDirectoryGroup ), typeof( string ), typeof( CommonItemDialogMessage ), new PropertyMetadata( "" ) );
		#endregion


		#region Register Title
		/// <summary>
		/// タイトルを設定または取得します。
		/// </summary>
		public string Title {
			get => (string)GetValue( TitleProperty );
			set => SetValue( TitleProperty, value );
		}
		
		/// <summary>
		/// <see cref="Title"/> 依存関係プロパティ
		/// </summary>
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
	}
}