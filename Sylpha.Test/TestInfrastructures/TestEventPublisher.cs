using System;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Sylpha.NUnit {
	public class TestEventPublisher : INotifyPropertyChanged, INotifyCollectionChanged {
		public EventHandler? EmptyEvent;
		public EventHandler<StringEventArgs>? StringEvent;

		public event PropertyChangedEventHandler? PropertyChanged;
		public event NotifyCollectionChangedEventHandler? CollectionChanged;

		public object? Dummy2 { get; set; }

		public void RaiseEmptyEvent() {
			EmptyEvent?.Invoke( this, EventArgs.Empty );
		}

		public void RaiseStringEvent( string arg ) {
			StringEvent?.Invoke( this, new StringEventArgs( arg ) );
		}

		public void RaisePropertyChanged( string? propertyName ) {
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
		}

		public void RaiseCollectionChanged( NotifyCollectionChangedAction action, object? item ) {
			CollectionChanged?.Invoke( this, new NotifyCollectionChangedEventArgs( action, item ) );
		}
	}

	public class StringEventArgs( string message ) : EventArgs {
		public string Message { get; private set; } = message;
	}
}
