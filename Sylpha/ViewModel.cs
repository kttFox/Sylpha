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
		public CompositeDisposable CompositeDisposable {
			get => _compositeDisposable ??= [];
			set => _compositeDisposable = value;
		}
		[NonSerialized] private CompositeDisposable? _compositeDisposable;

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
					_compositeDisposable?.Dispose();
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