namespace Sylpha.Messaging {
	/// <summary>
	/// WindowActionMessageで使用する、Windowが遷移すべき状態を表します。
	/// </summary>
	public enum WindowActionMode {
		/// <summary>
		/// ウィンドウを閉じます。
		/// </summary>
		Close,
		/// <summary>
		/// ウィンドウを最大化します。
		/// </summary>
		Maximize,
		/// <summary>
		/// ウィンドウを最小化します。
		/// </summary>
		Minimize,
		/// <summary>
		/// ウィンドウを通常状態にします。
		/// </summary>
		Normal,
		/// <summary>
		/// ウィンドウをアクティブにします。
		/// </summary>
		Active,
		/// <summary>
		/// ウィンドウを非表示にします。
		/// </summary>
		Hide,

		/// <summary>
		/// ウィンドウのDialogResultをTrueにします。
		/// </summary>
		ResultOK,
		/// <summary>
		/// ウィンドウのDialogResultをFalseにします。
		/// </summary>
		ResultCancel,
	}
}