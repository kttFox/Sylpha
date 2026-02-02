using System.Windows;

namespace Sylpha.Messaging {
	/// <summary>
	/// ファイルを開く、保存するアクション用の共通メッセージ基底抽象クラスです。
	/// </summary>
	public abstract class FileDialogMessage : RequestMessage<string[]> {
		#region Register Title
		/// <summary>
		/// ダイアログタイトルを指定、または取得します。
		/// </summary>
		public string? Title {
			get { return (string?)GetValue( TitleProperty ); }
			set { SetValue( TitleProperty, value ); }
		}

		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register( nameof( Title ), typeof( string ), typeof( FileDialogMessage ), new PropertyMetadata( null ) );
		#endregion

		#region Register InitialDirectory
		/// <summary>
		/// ダイアログに表示される初期ディレクトリを指定、または取得します。
		/// </summary>
		public string? InitialDirectory {
			get { return (string?)GetValue( InitialDirectoryProperty ); }
			set { SetValue( InitialDirectoryProperty, value ); }
		}

		public static readonly DependencyProperty InitialDirectoryProperty =
			DependencyProperty.Register( nameof( InitialDirectory ), typeof( string ), typeof( FileDialogMessage ), new PropertyMetadata( null ) );
		#endregion

		#region Register FilterIndex
		public int FilterIndex {
			get { return (int)GetValue( FilterIndexProperty ); }
			set { SetValue( FilterIndexProperty, value ); }
		}

		public static readonly DependencyProperty FilterIndexProperty =
			DependencyProperty.Register( nameof( FilterIndex ), typeof( int ), typeof( FileDialogMessage ), new PropertyMetadata( 1 ) );
		#endregion

		#region Register Filter
		/// <summary>
		/// ファイルの拡張子Filterを指定、または取得します。
		/// </summary>
		public string? Filter {
			get { return (string?)GetValue( FilterProperty ); }
			set { SetValue( FilterProperty, value ); }
		}

		public static readonly DependencyProperty FilterProperty =
			DependencyProperty.Register( nameof( Filter ), typeof( string ), typeof( FileDialogMessage ), new PropertyMetadata( null ) );
		#endregion

		#region Register FileName
		/// <summary>
		/// ファイルダイアログで指定されたファイルのパスを含む文字列を指定、または取得します。
		/// </summary>
		public string? FileName {
			get { return (string?)GetValue( FileNameProperty ); }
			set { SetValue( FileNameProperty, value ); }
		}

		public static readonly DependencyProperty FileNameProperty =
			DependencyProperty.Register( nameof( FileName ), typeof( string ), typeof( FileDialogMessage ), new PropertyMetadata( null ) );
		#endregion

		#region Register DefaultExt
		/// <summary>
		/// ファイル ダイアログで現在選択されているフィルターのインデックスを取得または設定します。 既定値は1です。
		/// </summary>
		/// <summary>
		/// 既定のファイル名の拡張子を取得または設定します。
		/// </summary>
		public string DefaultExt {
			get { return (string)GetValue( DefaultExtProperty ); }
			set { SetValue( DefaultExtProperty, value ); }
		}

		public static readonly DependencyProperty DefaultExtProperty =
			DependencyProperty.Register( nameof(DefaultExt), typeof( string ), typeof( FileDialogMessage ), new PropertyMetadata( string.Empty ) );
		#endregion

		#region Register CheckPathExists
		/// <summary>
		/// ユーザーが無効なパスとファイル名を入力した場合に警告を表示するかどうかを指定する値を取得または設定します。
		/// </summary>
		/// <returns>警告を表示する場合は <see langword="true"/>。それ以外の場合は <see langword="false"/>。 既定値は <see langword="true"/> です。</returns>
		public bool CheckPathExists {
			get => (bool)GetValue( CheckPathExistsProperty );
			set => SetValue( CheckPathExistsProperty, value );
		}

		public static readonly DependencyProperty CheckPathExistsProperty =
			DependencyProperty.Register( nameof( CheckPathExists ), typeof( bool ), typeof( FileDialogMessage ), new PropertyMetadata( true ) );
		#endregion

		#region Register CheckFileExists
		/// <summary>
		/// 存在しないファイル名をユーザーが指定した場合に、ファイル ダイアログで警告を表示するかどうかを示す値を取得または設定します。
		/// </summary>
		/// <returns>警告を表示する場合は <see langword="true"/>。それ以外の場合は <see langword="false"/>。 この基本クラスの既定値は <see langword="false"/> です。</returns>
		public bool CheckFileExists {
			get => (bool)GetValue( CheckFileExistsProperty );
			set => SetValue( CheckFileExistsProperty, value );
		}

		public static readonly DependencyProperty CheckFileExistsProperty =
			DependencyProperty.Register( nameof( CheckFileExists ), typeof( bool ), typeof( FileDialogMessage ), new PropertyMetadata( false ) );
		#endregion

		#region Register AddExtensionProperty
		/// <summary>
		/// 拡張子を指定しなかった場合、自動で拡張子を追加するかどうかを指定、または取得します。デフォルトはtrueです。
		/// </summary>
		public bool AddExtension {
			get { return (bool)( GetValue( AddExtensionProperty ) ); }
			set { SetValue( AddExtensionProperty, value ); }
		}

		public static readonly DependencyProperty AddExtensionProperty =
			DependencyProperty.Register( nameof( AddExtension ), typeof( bool ), typeof( FileDialogMessage ), new PropertyMetadata( true ) );
		#endregion

		/// <summary>
		/// 新しいメッセージのインスタンスを生成します。
		/// </summary>
		protected FileDialogMessage() { }

		/// <summary>
		/// メッセージキーを指定して、新しいメッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		protected FileDialogMessage( string? messageKey ) : base( messageKey ) { }

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br />
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected abstract override Freezable CreateInstanceCore();
	}
}