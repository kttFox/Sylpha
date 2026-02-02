using System.Windows;

namespace Sylpha.Messaging {
	/// <summary>
	/// ファイルを開く・ファイルを保存するアクション用の共通メッセージ基底抽象クラスです。<br />
	/// ファイルを開くアクションをViewに行わせたい場合は、<see cref="FileSelectionMessage" />を使用してください。<br />
	/// ファイルを保存するアクションをViewに行わせたい場合は、<see cref="SavingFileSelectionMessage" />を使用してください。
	/// </summary>
	public abstract class FileSelectionMessage : RequestMessage<string[]> {
		protected FileSelectionMessage() { }

		protected FileSelectionMessage( string? messageKey ) : base( messageKey ) { }

		#region TitleProperty
		/// <summary>
		/// ダイアログタイトルを指定、または取得します。
		/// </summary>
		public string? Title {
			get { return (string?)GetValue( TitleProperty ); }
			set { SetValue( TitleProperty, value ); }
		}

		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register( nameof( Title ), typeof( string ), typeof( FileSelectionMessage ), new PropertyMetadata( null ) );
		#endregion

		#region FilterProperty
		/// <summary>
		/// ファイルの拡張子Filterを指定、または取得します。
		/// </summary>
		public string? Filter {
			get { return (string?)GetValue( FilterProperty ); }
			set { SetValue( FilterProperty, value ); }
		}

		public static readonly DependencyProperty FilterProperty =
			DependencyProperty.Register( nameof( Filter ), typeof( string ), typeof( FileSelectionMessage ), new PropertyMetadata( null ) );
		#endregion

		#region AddExtensionProperty
		/// <summary>
		/// 拡張子を指定しなかった場合、自動で拡張子を追加するかどうかを指定、または取得します。デフォルトはtrueです。
		/// </summary>
		public bool AddExtension {
			get { return (bool)( GetValue( AddExtensionProperty ) ); }
			set { SetValue( AddExtensionProperty, value ); }
		}

		public static readonly DependencyProperty AddExtensionProperty =
			DependencyProperty.Register( nameof( AddExtension ), typeof( bool ), typeof( FileSelectionMessage ), new PropertyMetadata( true ) );
		#endregion

		#region InitialDirectoryProperty
		/// <summary>
		/// ダイアログに表示される初期ディレクトリを指定、または取得します。
		/// </summary>
		public string? InitialDirectory {
			get { return (string?)GetValue( InitialDirectoryProperty ); }
			set { SetValue( InitialDirectoryProperty, value ); }
		}

		public static readonly DependencyProperty InitialDirectoryProperty =
			DependencyProperty.Register( nameof( InitialDirectory ), typeof( string ), typeof( FileSelectionMessage ), new PropertyMetadata( null ) );
		#endregion

		#region FileNameProperty
		/// <summary>
		/// ファイルダイアログで指定されたファイルのパスを含む文字列を指定、または取得します。
		/// </summary>
		public string? FileName {
			get { return (string?)GetValue( FileNameProperty ); }
			set { SetValue( FileNameProperty, value ); }
		}

		public static readonly DependencyProperty FileNameProperty =
			DependencyProperty.Register( nameof( FileName ), typeof( string ), typeof( FileSelectionMessage ), new PropertyMetadata( null ) );
		#endregion
		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected abstract override Freezable CreateInstanceCore();
	}
}