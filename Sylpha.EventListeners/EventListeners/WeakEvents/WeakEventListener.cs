
using System;
using System.Diagnostics.CodeAnalysis;

namespace Sylpha.EventListeners.WeakEvents {
	/// <summary>
	/// 汎用WeakEventリスナーオブジェクト
	/// </summary>
	/// <typeparam name="THandler">対象のイベントのイベント ハンドラー型</typeparam>
	/// <typeparam name="TEventArgs">対象のイベントのイベント引数型</typeparam>
	public class WeakEventListener<THandler, TEventArgs> : IDisposable where THandler : Delegate where TEventArgs : EventArgs {
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に非 null 値が入っていなければなりません。'required' 修飾子を追加するか、null 許容として宣言することを検討してください。
		/// <summary>
		/// 継承先ではInitializeを呼び出して初期化を行ってください。
		/// </summary>
		protected WeakEventListener() { }
#pragma warning restore CS8618

		/// <summary>
		/// <see cref="WeakEventListener{THandler, TEventArgs}"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="conversion">ジェネリックイベント ハンドラー型を<typeparamref name="THandler"/>型に変換する <see cref="Func{T, TResult}"/> デリゲート</param>
		/// <param name="add">h => obj.Event += h の様な形でイベントの購読を登録するための <see cref="Action{T}"/> デリゲート</param>
		/// <param name="remove">h => obj.Event += h の様な形でイベントの購読を登録するための <see cref="Action{T}"/> デリゲート</param>
		/// <param name="handler">イベントを受信した際に実行される <typeparamref name="THandler"/> デリゲート</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c> の場合に発生します。</exception>
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

		/// <summary>
		/// <see cref="WeakEventListener{THandler, TEventArgs}"/> を初期化します。
		/// </summary>
		/// <param name="conversion">ジェネリックイベント ハンドラー型を<typeparamref name="THandler"/>型に変換する <see cref="Func{T, TResult}"/> デリゲート</param>
		/// <param name="add">h => obj.Event += h の様な形でイベントの購読を登録するための <see cref="Action{T}"/> デリゲート</param>
		/// <param name="remove">h => obj.Event += h の様な形でイベントの購読を登録するための <see cref="Action{T}"/> デリゲート</param>
		/// <param name="handler">イベントを受信した際に実行される <typeparamref name="THandler"/> デリゲート</param>
		/// <exception cref="Exception">すでに<see cref="Initialize"/>が呼び出されている場合に発生します。</exception>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c> の場合に発生します。</exception>
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

		/// <summary>
		/// イベントソースとの接続を解除します。
		/// </summary>
		public virtual void Dispose() {
			if( !_disposed ) {
				_disposed = true;

				_remove( _attachHandler );
				_handler = default!;
				_remove = default!;
				_attachHandler = default!;
			}
		}

		/// <summary>
		/// このオブジェクトが破棄されている場合、ObjectDisposedException を発生させます。
		/// </summary>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		protected void ThrowExceptionIfDisposed() {
			if( _disposed ) throw new ObjectDisposedException( "WeakEventListener" );
		}
		#endregion
	}
}