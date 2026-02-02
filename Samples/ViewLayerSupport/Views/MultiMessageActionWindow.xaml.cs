using System.Windows;
using Sylpha.Messaging.Behaviors;

namespace ViewLayerSupport.Views {
	/// <summary>
	/// MultiMessageActionWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MultiMessageActionWindow : Window {
		public MultiMessageActionWindow() {
			InitializeComponent();

#if NET8_0_OR_GREATER
			this.MessageTrigger.Actions.Add( new OpenFolderDialogMessageAction() );
#endif
		}

		private string GetText() {
			MessageBox.Show( this, "This text is from MultiMessageActionWindow." );
			return "This text is from MultiMessageActionWindow.";
		}
	}
}