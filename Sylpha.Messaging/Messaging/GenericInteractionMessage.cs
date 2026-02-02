using System.Windows;

namespace Sylpha.Messaging {
	public class GenericInteractionMessage<T> : InteractionMessage {
		// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register( nameof( Value ), typeof( T ), typeof( GenericInteractionMessage<T> ), new PropertyMetadata( default( T ) ) );

		public GenericInteractionMessage( T value, string? messageKey = null ) : base( messageKey ) {
			Value = value;
		}

		public T Value {
			get { return (T)GetValue( ValueProperty ); }
			set { SetValue( ValueProperty, value ); }
		}

		protected override Freezable CreateInstanceCore() {
			return new GenericInteractionMessage<T>( Value, MessageKey );
		}
	}
}