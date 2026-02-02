namespace Sylpha.Messaging {
	public enum ShowWindowMode {
		/// <summary>
		/// 新しいWindowをモーダルウインドウとして開きます。
		/// </summary>
		Modal,
		/// <summary>
		/// 新しいWindowをモーダレスウインドウとして開きます。
		/// </summary>
		Modeless,
		/// <summary>
		/// すでに同じ型のWindowが開かれている場合はそのWindowをアクティブにします。<br />
		/// 同じ型のWindowが開かれていなかった場合、新しくWindowを開きます。
		/// </summary>
		NewOrActive,
	}
}