using System;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;

namespace Sylpha.Messaging.Behaviors {
	/// <summary>
	/// 画面遷移(Window)を行うアクションです。<see cref="TransitionMessage" />に対応します。
	/// </summary>
	public class TransitionMessageAction : MessageAction<FrameworkElement> {

		#region Register WindowTypeProperty
		/// <summary>
		/// 遷移するウインドウの型を指定、または取得します。
		/// </summary>
		public Type? WindowType {
			get { return (Type?)GetValue( WindowTypeProperty ); }
			set { SetValue( WindowTypeProperty, value ); }
		}

		public static readonly DependencyProperty WindowTypeProperty =
					DependencyProperty.Register( nameof( WindowType ), typeof( Type ), typeof( TransitionMessageAction ), new PropertyMetadata() );
		#endregion

		#region Register ModeProperty
		/// <summary>
		/// 画面遷移の種類を指定するTransitionMode列挙体を指定、または取得します。<br />
		/// TransitionMessageでModeがUnKnown以外に指定されていた場合、そちらが優先されます。
		/// </summary>
		public TransitionMode Mode {
			get { return (TransitionMode)( GetValue( ModeProperty ) ); }
			set { SetValue( ModeProperty, value ); }
		}

		public static readonly DependencyProperty ModeProperty =
					DependencyProperty.Register( nameof( Mode ), typeof( TransitionMode ), typeof( TransitionMessageAction ), new PropertyMetadata( TransitionMode.Normal ) );
		#endregion

		#region Register OwnedFromThisProperty
		/// <summary>
		/// 遷移先ウィンドウがこのウィンドウに所有されるかを設定します。
		/// </summary>
		public bool OwnedFromThis {
			get { return (bool)( GetValue( OwnedFromThisProperty ) ); }
			set { SetValue( OwnedFromThisProperty, value ); }
		}

		public static readonly DependencyProperty OwnedFromThisProperty =
					DependencyProperty.Register( nameof( OwnedFromThis ), typeof( bool ), typeof( TransitionMessageAction ), new PropertyMetadata( true ) );
		#endregion

		private static bool IsValidWindowType( Type value ) {
			return value != null
				   && value.IsSubclassOf( typeof( Window ) )
				   && value.GetConstructor( Type.EmptyTypes ) != null;
		}

		protected override void InvokeAction( Message message ) {
			if( message is not TransitionMessage transitionMessage ) return;

			var targetType = transitionMessage.WindowType ?? WindowType;
			if( targetType == null ) return;
			if( !IsValidWindowType( targetType ) ) return;

			var defaultConstructor = targetType.GetConstructor( Type.EmptyTypes )
									 ?? throw new InvalidOperationException();

			if( Mode == TransitionMode.UnKnown && transitionMessage.Mode == TransitionMode.UnKnown ) return;

			var mode = transitionMessage.Mode == TransitionMode.UnKnown ? Mode : transitionMessage.Mode;
			var associatedObject = AssociatedObject ?? throw new InvalidOperationException( $"{nameof( AssociatedObject )} cannot be null." );

			switch( mode ) {
				case TransitionMode.Normal:
				case TransitionMode.Modal:
					var targetWindow = (Window)defaultConstructor.Invoke( null )
									   ?? throw new InvalidOperationException();

					if( transitionMessage.TransitionViewModel != null )
						targetWindow.DataContext = transitionMessage.TransitionViewModel;

					if( this.OwnedFromThis ) {
						targetWindow.Owner = Window.GetWindow( associatedObject );
					}

					if( mode == TransitionMode.Normal ) {
						targetWindow.Show();
						transitionMessage.Response = null;
					} else
						transitionMessage.Response = targetWindow.ShowDialog();

					break;
				case TransitionMode.NewOrActive:
					var window = Application.Current?.Windows.OfType<Window>()
						.FirstOrDefault( w => w.GetType() == targetType );

					if( window == null ) {
						window = (Window)defaultConstructor.Invoke( null )
								 ?? throw new InvalidOperationException();

						if( transitionMessage.TransitionViewModel != null )
							window.DataContext = transitionMessage.TransitionViewModel;

						if( this.OwnedFromThis ) {
							window.Owner = Window.GetWindow( associatedObject );
						}

						window.Show();
						transitionMessage.Response = null;
					} else {
						if( transitionMessage.TransitionViewModel != null )
							window.DataContext = transitionMessage.TransitionViewModel;

						if( this.OwnedFromThis ) {
							window.Owner = Window.GetWindow( associatedObject );
						}

						window.Activate();

						// 最小化中なら戻す
						if( window.WindowState == WindowState.Minimized ) window.WindowState = WindowState.Normal;

						transitionMessage.Response = null;
					}

					break;
				case TransitionMode.UnKnown:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}