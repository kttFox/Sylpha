using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Sylpha.EventListeners.WeakEvents {
	/// <summary>
	/// <see cref="INotifyPropertyChanged.PropertyChanged"/> イベントを受信するためのWeakイベントリスナー
	/// </summary>
	public sealed class PropertyChangedWeakEventListener : WeakEventListener<PropertyChangedEventHandler, PropertyChangedEventArgs>, IEnumerable<KeyValuePair<string, List<PropertyChangedEventHandler>>>, IDisposable {
		private readonly WeakReference<INotifyPropertyChanged> _source;
		private readonly Dictionary<string, List<PropertyChangedEventHandler>> _handlerDictionary = [];

		/// <summary>
		/// <see cref="PropertyChangedEventListener"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="source">イベントを受信する対象のオブジェクト</param>
		/// <exception cref="ArgumentNullException"><paramref name="source"/> が <c>null</c> の場合に発生します。</exception>
		public PropertyChangedWeakEventListener( INotifyPropertyChanged source ) {
			if( source == null ) throw new ArgumentNullException( nameof( source ) );
			_source = new( source );

			var _this = this;
			Initialize(
				h => new PropertyChangedEventHandler( h ),
				h => source.PropertyChanged += h,
				h => source.PropertyChanged -= h,
				( sender, e ) => _this.ExecuteHandler( e )
			);
		}

		void ExecuteHandler( PropertyChangedEventArgs e ) {
			if( _source.TryGetTarget( out var sourceResult ) ) {
				if( e.PropertyName != null ) {
					if( _handlerDictionary.TryGetValue( e.PropertyName, out var list ) ) {
						foreach( var handler in list ) {
							handler( sourceResult, e );
						}
					}
				}

				{
					if( _handlerDictionary.TryGetValue( string.Empty, out var allList ) ) {
						foreach( var handler in allList ) {
							handler( sourceResult, e );
						}
					}
				}
			}
		}

		IEnumerator<KeyValuePair<string, List<PropertyChangedEventHandler>>> IEnumerable<KeyValuePair<string, List<PropertyChangedEventHandler>>>.GetEnumerator()
			=> _handlerDictionary.GetEnumerator();


		IEnumerator IEnumerable.GetEnumerator()
			=> _handlerDictionary.GetEnumerator();


		/// <summary>
		/// 共通のイベント ハンドラーを登録します。
		/// </summary>
		/// <param name="handlers">登録するイベント ハンドラー</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c> 、または <c>null</c> を含む場合に発生します。</exception>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		public void RegisterHandler( params IEnumerable<PropertyChangedEventHandler> handlers ) {
			if( handlers == null || handlers.Contains( null ) ) throw new ArgumentNullException( nameof( handlers ) );
			ThrowExceptionIfDisposed();

			RegisterHandler( string.Empty, handlers );
		}

		/// <summary>
		/// プロパティ名でフィルタリング済のイベント ハンドラーを登録します。
		/// </summary>
		/// <param name="propertyName">イベント ハンドラーを登録したいプロパティの名前</param>
		/// <param name="handlers">登録するイベント ハンドラー</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c> 、または <c>null</c> を含む場合に発生します。</exception>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		public void RegisterHandler( string propertyName, params IEnumerable<PropertyChangedEventHandler> handlers ) {
			if( propertyName == null ) throw new ArgumentNullException( nameof( propertyName ) );
			if( handlers == null || handlers.Contains( null ) ) throw new ArgumentNullException( nameof( handlers ) );
			ThrowExceptionIfDisposed();

			if( !_handlerDictionary.TryGetValue( propertyName, out var list ) ) {
				_handlerDictionary[propertyName] = list = [];
			}
			list.AddRange( handlers );
		}

		/// <summary>
		/// プロパティ名でフィルタリング済のイベント ハンドラーを追加します。
		/// </summary>
		/// <param name="propertyExpression">イベント ハンドラーを登録したいプロパティの名前。 () => プロパティ形式のラムダ式</param>
		/// <param name="handlers">登録するイベント ハンドラー</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c> 、または <c>null</c> を含む場合に発生します。</exception>
		/// <exception cref="NotSupportedException"><paramref name="propertyExpression"/> が ()=>プロパティ の形式のラムダ式 以外場合に発生します。</exception>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		public void RegisterHandler<T>( Expression<Func<T>> propertyExpression, params IEnumerable<PropertyChangedEventHandler> handlers ) {
			if( propertyExpression == null ) throw new ArgumentNullException( nameof( propertyExpression ) );
			if( handlers == null || handlers.Contains( null ) ) throw new ArgumentNullException( nameof( handlers ) );
			if( ( (MemberExpression)propertyExpression.Body ) is not MemberExpression memberExpression ) {
				throw new NotSupportedException( "このメソッドでは ()=>プロパティ の形式のラムダ式以外許可されません" );
			}
			ThrowExceptionIfDisposed();

			RegisterHandler( memberExpression.Member.Name, handlers );
		}

		/// <summary>
		/// 共通のイベント ハンドラーを登録します。
		/// </summary>
		/// <param name="handler">登録するイベント ハンドラー</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c> 、または <c>null</c> を含む場合に発生します。</exception>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		public void Add( PropertyChangedEventHandler handler ) => this.RegisterHandler( handler );
		/// <summary>
		/// プロパティ名でイベント ハンドラーを登録します。
		/// </summary>
		/// <param name="propertyName">イベント ハンドラーを登録したいプロパティの名前</param>
		/// <param name="handler">登録するイベント ハンドラー</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c> 、または <c>null</c> を含む場合に発生します。</exception>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		public void Add( string propertyName, PropertyChangedEventHandler handler ) => this.RegisterHandler( propertyName, handler );
		/// <summary>
		/// プロパティ名でイベント ハンドラーを登録します。
		/// </summary>
		/// <param name="propertyName">イベント ハンドラーを登録したいプロパティの名前</param>
		/// <param name="handlers">登録するイベント ハンドラー</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c> 、または <c>null</c> を含む場合に発生します。</exception>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		public void Add( string propertyName, IEnumerable<PropertyChangedEventHandler> handlers ) => this.RegisterHandler( propertyName, handlers );
		/// <summary>
		/// プロパティ名でイベント ハンドラーを登録します。
		/// </summary>
		/// <param name="propertyExpression">イベント ハンドラーを登録したいプロパティの名前。 () => プロパティ形式のラムダ式</param>
		/// <param name="handler">登録するイベント ハンドラー</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c> 、または <c>null</c> を含む場合に発生します。</exception>
		/// <exception cref="NotSupportedException"><paramref name="propertyExpression"/> が ()=>プロパティ の形式のラムダ式 以外場合に発生します。</exception>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		public void Add<T>( Expression<Func<T>> propertyExpression, PropertyChangedEventHandler handler ) => this.RegisterHandler( propertyExpression, handler );
		/// <summary>
		/// プロパティ名でイベント ハンドラーを登録します。
		/// </summary>
		/// <param name="propertyExpression">イベント ハンドラーを登録したいプロパティの名前。 () => プロパティ形式のラムダ式</param>
		/// <param name="handlers">登録するイベント ハンドラー</param>
		/// <exception cref="ArgumentNullException">引数 が <c>null</c> 、または <c>null</c> を含む場合に発生します。</exception>
		/// <exception cref="NotSupportedException"><paramref name="propertyExpression"/> が ()=>プロパティ の形式のラムダ式 以外場合に発生します。</exception>
		/// <exception cref="ObjectDisposedException">このオブジェクトが既に破棄されている場合に発生します。</exception>
		public void Add<T>( Expression<Func<T>> propertyExpression, IEnumerable<PropertyChangedEventHandler> handlers ) => this.RegisterHandler( propertyExpression, handlers );
	}
}