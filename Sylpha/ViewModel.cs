using System;
using System.Xml.Serialization;
using Sylpha.Messaging;

namespace Sylpha {
	/// <summary>
	/// ViewModelの基底クラスです。
	/// </summary>
	[Serializable]
	public abstract class ViewModel : NotificationObject, IDisposable {

		public virtual void Initialize() { }

		/// <summary>
		/// このViewModelクラスの基本CompositeDisposableです。
		/// </summary>
		[XmlIgnore]
		public DisposableCollection DisposableCollection {
			get => _disposableCollection ??= [];
			set => _disposableCollection = value;
		}
		[NonSerialized] private DisposableCollection? _disposableCollection;

		/// <summary>
		/// このViewModelクラスの基本Messengerインスタンスです。
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