using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Sylpha.EventListeners.WeakEvents {
	/// <summary>
	/// 汎用WeakEventリスナーオブジェクトです。
	/// </summary>
	/// <typeparam name="THandler">対象のイベントのイベントハンドラ型</typeparam>
	/// <typeparam name="TEventArgs">対象のイベントのイベント引数型</typeparam>
	public class WeakEventListener<THandler, TEventArgs> : IDisposable where THandler : Delegate where TEventArgs : EventArgs {

#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に非 null 値が入っていなければなりません。'required' 修飾子を追加するか、null 許容として宣言することを検討してください。
		/// <summary>
		/// 継承先ではInitializeを呼び出して初期化を行ってください。
		/// </summary>
		protected WeakEventListener() { }
#pragma warning restore CS8618

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="conversion">ジェネリックイベントハンドラ型をTHandler方に変換するFunc</param>
		/// <param name="add">h => obj.Event += > h の様な形でイベントの購読を登録するためのAction。hはTHandler型です。</param>
		/// <param name="remove">h => obj.Event += > h の様な形でイベントの購読を登録するためのAction。hはTHandler型です。</param>
		/// <param name="handler">イベントを受信した際に行いたいアクション</param>
		public WeakEventListener( Func<EventHandler<TEventArgs>, THandler> conversion, Action<THandler> add, Action<THandler> remove, EventHandler<TEventArgs> handler ) {
			if( conversion == null ) throw new ArgumentNullException( nameof( conversion ) );
			if( add == null ) throw new ArgumentNullException( nameof( add ) );
			if( remove == null ) throw new ArgumentNullException( nameof( remove ) );
			if( handler == null ) throw new ArgumentNullException( nameof( handler ) );

			Initialize( conversion, add, remove, handler );
		}

		private bool _initialized;

		private EventHandler<TEventArgs> _handler;
		private Action<THandler> _remove;
		private THandler _attachHandler;

		[MemberNotNull( nameof( _handler ), nameof( _remove ), nameof( _attachHandler ) )]
		protected void Initialize( Func<EventHandler<TEventArgs>, THandler> conversion, Action<THandler> add, Action<THandler> remove, EventHandler<TEventArgs> handler ) {
			if( _initialized ) throw new Exception( "すでにInitialize済みです。" );
			_initialized = true;

			if( conversion == null ) throw new ArgumentNullException( nameof( conversion ) );
			if( add == null ) throw new ArgumentNullException( nameof( add ) );

			var weakReference = new WeakReference<WeakEventListener<THandler, TEventArgs>>( this );
			_handler = handler ?? throw new ArgumentNullException( nameof( handler ) );
			_remove = remove ?? throw new ArgumentNullException( nameof( remove ) );

			THandler attachHandler = null!;
			_attachHandler = attachHandler = conversion( ( sender, e ) => {
				if( weakReference.TryGetTarget( out var listener ) ) {
					listener._handler( sender, e );
				} else {
					remove( attachHandler );
				}
			} );

			add( _attachHandler );
		}

		#region Dispose
		private bool _disposed;

		public virtual void Dispose() {
			if( !_disposed ) {
				_disposed = true;

					_remove( _attachHandler );
					_handler = default!;
					_remove = default!;
					_attachHandler = default!;
				}
		}

		protected void ThrowExceptionIfDisposed() {
			if( _disposed ) throw new ObjectDisposedException( "WeakEventListener" );
		}
		#endregion
	}
}