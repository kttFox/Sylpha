using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Sylpha;
using Sylpha.Commands;
using Sylpha.EventListeners;
using Sylpha.Messaging;

namespace ViewLayerSupport.ViewModels {
	public class CommandWindowViewModel : ViewModel {
		public CommandWindowViewModel() {
			new PropertyChangedEventListener( this ) { 
				{ nameof( Command2Flag ), ( _, _ ) => { this.Command2.RaiseCanExecuteChanged(); } }
			}.AddTo( this.DisposableCollection );
		}

		public string? Text { get; set => SetProperty( ref field, value ); }

		public bool Command1Flag {
			get;
			set {
				if( SetProperty( ref field, value ) ) {
					this.Command1.RaiseCanExecuteChanged();
				}
			}
		} = true;

		#region Command1
		public DelegateCommand Command1 => field ??= new( DoCommand1, CanCommand1 );

		private bool CanCommand1( ) {
			return Command1Flag;
		}

		private void DoCommand1() {
			this.Text = "Command1 Clicked";
		}
		#endregion


		public bool Command2Flag { get; set => SetProperty( ref field, value ); } 

		#region Command2
		public DelegateCommand<string> Command2 => field ??= new( DoCommand2, CanCommand2 );

		private bool CanCommand2( string? value ) {
			return Command2Flag;
		}

		private void DoCommand2( string? value ) {
			this.Text = value;
		}
		#endregion

	}
}
