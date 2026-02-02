using System;
using System.Windows;
using ViewLayerSupport.ViewModels;

namespace ViewLayerSupport.Views {
	/// <summary>
	/// CallMethodActionWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class CallMethodActionWindow : Window {
		public CallMethodActionWindow() {
			InitializeComponent();
		}

		void ButtonClick() {
			var vm = (CallMethodActionWindowViewModel)DataContext;
			vm.Text = $"[{DateTime.Now}] View - Button Clicked!";
		}

		void ButtonClick( string? param ) {
			var vm = (CallMethodActionWindowViewModel)DataContext;
			vm.Text = $"[{DateTime.Now}] View - Button Clicked with param: {param ?? "null"}";
		}

		string GetText() => "Hello from View!";

		string GetText( string value ) => $"Hello from View with param: {value}";
	}
}