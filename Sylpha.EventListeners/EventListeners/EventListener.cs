using System;
using System.Diagnostics.CodeAnalysis;

namespace Sylpha.EventListeners {
	/// <summary>
	/// 汎用イベントリスナーオブジェクト
	/// </summary>
	/// <typeparam name="THandler">イベント ハンドラーの型</typeparam>
	public class EventListener<THandler> : IDisposable where THandler : Delegate {
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に非 null 値が入っていなければなりません。'required' 修飾子を追加するか、null 許容として宣言することを検討してください。
		/// <summary>
		/// 継承先では初期化を行うために <see cref="Initialize"/> を呼び出す必要があります。
		/// </summary>
		protected EventListener() { }
#pragma warning restore CS8618

		/// <summary>
		/// <see cref="EventListener{THandler}"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="add">h => obj.Event += h の形式でイベントの購読を登録するための <see cref="Action{T}"/> デリゲート</param>
		/// <param name="remove">h => obj.Event -= h の形式でイベントの購読を解除するための <see cref="Action{T}"/> デリゲート</param>
		/// <param name="handler">イベントを受信した際に実行される <typeparamref name="THandler"/> デリゲート</param>
		/// <exception cref="ArgumentNullException">いずれかの引数が <c>null</c> の場合に発生します。</exception>
		public EventListener( Action<THandler> add, Action<THandler> remove, THandler handler ) {
			if( add == null ) throw new ArgumentNullException( nameof( add ) );
			if( remove == null ) throw new ArgumentNullException( nameof( remove ) );
			if( handler == null ) throw new ArgumentNullException( nameof( handler ) );

			Initialize( add, remove, handler );
		}

		private bool _initialized;

		private Action<THandler> _remove;
		private THandler _handler;

		/// <summary>
		/// <see cref="EventListener{THandler}"/> を初期化します。
		/// </summary>
		/// <param name="add">h => obj.Event += h の形式でイベントの購読を登録するための <see cref="Action{T}"/> デリゲート</param>
		/// <param name="remove">h => obj.Event -= h の形式でイベントの購読を解除するための <see cref="Action{T}"/> デリゲート</param>
		/// <param name="handler">イベントを受信した際に実行される <typeparamref name="THandler"/> デリゲート</param>
		/// <exception cref="Exception">すでに初期化済みの場合に発生します。</exception>
		/// <exception cref="ArgumentNullException">いずれかの引数が <c>null</c> の場合に発生します。</exception>
		[MemberNotNull( nameof( _handler ), nameof( _remove ) )]
		protected void Initialize( Action<THandler> add, Action<THandler> remove, THandler handler ) {
			if( _initialized ) { throw new Exception( "すでに初期化済みです。" ); }
			_initialized = true;

			if( add == null ) throw new ArgumentNullException( nameof( add ) );
			_remove = remove ?? throw new ArgumentNullException( nameof( remove ) );
			_handler = handler ?? throw new ArgumentNullException( nameof( handler ) );

			add( handler );
		}

		#region Dispose
		private bool _disposed;

		/// <summary>
		/// イベント ハンドラーの登録を解除します。
		/// </summary>
		public virtual void Dispose() {
			if( !_disposed ) {
				_disposed = true;

				_remove( _handler );
				_remove = default!;
				_handler = default!;
			}
		}

		/// <summary>
		/// このオブジェクトが破棄されている場合、<see cref="ObjectDisposedException"/> を発生させます。
		/// </summary>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		protected void ThrowExceptionIfDisposed() {
			if( _disposed ) throw new ObjectDisposedException( "EventListener" );
		}

		#endregion
	}
}