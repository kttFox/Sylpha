using System;
using System.Xml.Serialization;
using Sylpha.Messaging;

namespace Sylpha {
	/// <summary>
	/// ViewModel の基底クラス
	/// </summary>
	[Serializable]
	public abstract class ViewModel : NotificationObject, IDisposable {
		/// <summary>
		/// View の <see cref="System.Windows.Window.ContentRendered"/> イベントに対応する初期化メソッド
		/// </summary>
		public virtual void Initialize() { }

		/// <summary>
		/// 破棄可能なオブジェクトのコレクションを取得または設定します。
		/// </summary>
		[XmlIgnore]
		public DisposableCollection DisposableCollection {
			get => _disposableCollection ??= [];
			set => _disposableCollection = value;
		}
		[NonSerialized] private DisposableCollection? _disposableCollection;

		/// <summary>
		/// <see cref="Sylpha.Messaging.Messenger"/> インスタンスを取得または設定します。
		/// </summary>
		[XmlIgnore]
		[field: NonSerialized]
		public Messenger Messenger {
			get => field ??= new Messenger();
			set;
		}

		#region Dispose
		[NonSerialized] private bool _disposed;

		/// <summary>
		/// このインスタンスによって使用されているすべてのリソースを解放します。
		/// </summary>
		public virtual void Dispose() {
			if( !_disposed ) {
				_disposed = true;

				_disposableCollection?.Dispose();
			}
		}
		#endregion
	}
}