using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel;

namespace Sylpha.NUnit {
	[TestFixture]
	public class NotificationObjectTest {
		[Test]
		public void RaisePropertyChangedIfSet() {
			var eventArgs = new List<PropertyChangedEventArgs>();
			var n = new RaisePropertyChangedIfSetTestObject();
			n.PropertyChanged += ( _, e ) => eventArgs.Add( e );

			n.Prop1 = "Changed";
			eventArgs.Count.Is( 1 );
			eventArgs[0].Is( x => x.PropertyName == nameof( RaisePropertyChangedIfSetTestObject.Prop1 ) );
		}

		class RaisePropertyChangedIfSetTestObject : NotificationObject {
			public string? Prop1 { get; set => SetProperty( ref field, value ); }
		}
	}
}
