using System.Windows;
using Sylpha.Messaging.Behaviors;

namespace ViewLayerSupport.Views {
	/// <summary>
	/// MessagingWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MessagingWindow : Window {
		public MessagingWindow() {
			InitializeComponent();

#if NET8_0_OR_GREATER
			this.MessageTrigger.Actions.Add( new OpenFolderDialogMessageAction() );
#endif
		}
	}
}