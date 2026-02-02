using System;
using System.Xml.Serialization;
using Sylpha.Messaging;

namespace Sylpha {
	/// <summary>
	/// ViewModelの基底クラスです。
	/// </summary>
	[Serializable]
	public abstract class ViewModel : NotificationObject, IDisposable {

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
		protected virtual void Dispose( bool disposing ) {
			if( !_disposed ) {
				_disposed = true;
				if( disposing ) {
					_disposableCollection?.Dispose();
				}
			}
		}

		[NonSerialized] private bool _disposed;

		/// <summary>
		/// このインスタンスによって使用されているすべてのリソースを解放します。
		/// </summary>
		public void Dispose() {
			Dispose( true );
		}
		#endregion
	}
}