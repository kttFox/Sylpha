using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using NUnit.Framework;

namespace Sylpha.NUnit {
	[TestFixture]
	public class NotificationObjectTest {
		[Test]
		public void PropertyChangedTest() {
			var Prop1Success = false;
			var Prop2Success = false;
			void Reset() {
				Prop1Success = false;
				Prop2Success = false;
			}

			var item = new Item();
			item.PropertyChanged += ( _, e ) => {
				switch( e.PropertyName ) {
					case nameof( Item.Prop1 ): {
						Prop1Success = true;
						break;
					}
					case nameof( Item.Prop2 ): {
						Prop2Success = true;
						break;
					}
				}
			};

			//------------------------
			Reset();

			item.Prop1 = "Changed";
			Prop1Success.Is( true );
			Prop2Success.Is( false );

			//------------------------
			Reset();

			item.Prop2 = "Changed";
			Prop1Success.Is( false );
			Prop2Success.Is( true );

			//------------------------
			Reset();

			item.Prop1 = "Changed";
			item.Prop2 = "Changed";
			Prop1Success.Is( false );
			Prop2Success.Is( false );
		}


		[Test]
		public void RaisePropertyChangedTest() {
			var Text1Success = false;
			var Text2Success = false;
			void Reset() {
				Text1Success = false;
				Text2Success = false;
			}

			var item = new Item();
			item.PropertyChanged += ( _, e ) => {
				switch( e.PropertyName ) {
					case nameof( Item.Text1 ): {
						Text1Success = true;
						break;
					}
					case nameof( Item.Text2 ): {
						Text2Success = true;
						break;
					}
				}
			};

			//------------------------
			Reset();

			item.Text = "Changed";
			Text1Success.Is( true );
			Text2Success.Is( true );

		}

		class Item : NotificationObject {
			public string? Prop1 { get; set => SetProperty( ref field, value ); }
			public string? Prop2 { get; set => SetProperty( ref field, value ); }

			public string? Text {
				get; set {
					if( SetProperty( ref field, value ) ) {
						this.RaisePropertyChanged( nameof( Text1 ) );
						this.RaisePropertyChanged( () => Text2 );
					}
				}
			}

			public string? Text1 => Text + "+1";
			public string? Text2 => Text + "+2";
		}
	}
}
