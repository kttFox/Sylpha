using System.Windows;

namespace Sylpha.Messaging {
	public class GenericResponsiveInteractionMessage<TValue, TResponse> : ResponsiveInteractionMessage<TResponse> {
		// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register( nameof( Value ), typeof( TValue ), typeof( GenericResponsiveInteractionMessage<TValue, TResponse> ), new PropertyMetadata( default( TValue ) ) );

		public GenericResponsiveInteractionMessage( TValue value, string? messageKey = null ) : base( messageKey ) {
			Value = value;
		}

		public TValue Value {
			get { return (TValue)GetValue( ValueProperty ); }
			set { SetValue( ValueProperty, value ); }
		}

		protected override Freezable CreateInstanceCore() {
			return new GenericResponsiveInteractionMessage<TValue, TResponse>( Value, MessageKey );
		}
	}
}