using System.Windows;

namespace Sylpha.Messaging {
	public class Message<TValue> : Message {
		#region Register ValueProperty
		/// <summary>
		/// 値を指定、または取得します。
		/// </summary>
		public TValue Value {
			get { return (TValue)GetValue( ValueProperty ); }
			set { SetValue( ValueProperty, value ); }
		}

		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register( nameof( Value ), typeof( TValue ), typeof( Message<TValue> ), new PropertyMetadata( default( TValue ) ) );
		#endregion

		public Message( TValue value, string? messageKey = null ) : base( messageKey ) {
			Value = value;
		}

		protected override Freezable CreateInstanceCore() {
			return new Message<TValue>( Value, MessageKey );
		}
	}
}