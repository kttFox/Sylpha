using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Sylpha.EventListeners.Internals;

namespace Sylpha.EventListeners {
	/// <summary>
	/// INotifyPropertyChanged.PropertyChangedを受信するためのイベントリスナーです。
	/// </summary>
	public sealed class PropertyChangedEventListener : EventListener<PropertyChangedEventHandler>, IEnumerable<KeyValuePair<string, List<PropertyChangedEventHandler>>> {
		private readonly PropertyChangedEventHandlerBag _bag;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">INotifyPropertyChangedオブジェクト</param>
		public PropertyChangedEventListener( INotifyPropertyChanged source ) {
			if( source == null ) throw new ArgumentNullException( nameof( source ) );

			_bag = new PropertyChangedEventHandlerBag( source );
			Initialize(
				h => source.PropertyChanged += h,
				h => source.PropertyChanged -= h,
				( sender, e ) => _bag.ExecuteHandler( e )
			);
		}

		IEnumerator<KeyValuePair<string, List<PropertyChangedEventHandler>>> IEnumerable<KeyValuePair<string, List<PropertyChangedEventHandler>>>.GetEnumerator() 
			=> _bag.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() 
			=> _bag.GetEnumerator();

		/// <summary>
		/// このリスナーインスタンスに新たなハンドラを追加します。
		/// </summary>
		/// <param name="handler">PropertyChangedイベントハンドラ</param>
		public void RegisterHandler( PropertyChangedEventHandler handler ) {
			if( handler == null ) throw new ArgumentNullException( nameof( handler ) );

			ThrowExceptionIfDisposed();
			_bag.RegisterHandler( handler );
		}

		/// <summary>
		/// このリスナーインスタンスにプロパティ名でフィルタリング済のハンドラを追加します。
		/// </summary>
		/// <param name="propertyName">ハンドラを登録したいPropertyChangedEventArgs.PropertyNameの名前</param>
		/// <param name="handlers">propertyNameで指定されたプロパティ用のPropertyChangedイベントハンドラ</param>
		public void RegisterHandler( string propertyName, params IEnumerable<PropertyChangedEventHandler> handlers ) {
			if( propertyName == null ) throw new ArgumentNullException( nameof( propertyName ) );
			if( handlers == null ) throw new ArgumentNullException( nameof( handlers ) );

			ThrowExceptionIfDisposed();
			_bag.RegisterHandler( propertyName, handlers );
		}

		/// <summary>
		/// このリスナーインスタンスにプロパティ名でフィルタリング済のハンドラを追加します。
		/// </summary>
		/// <param name="propertyExpression">() => プロパティ形式のラムダ式</param>
		/// <param name="handlers">propertyExpressionで指定されたプロパティ用のPropertyChangedイベントハンドラ</param>
		public void RegisterHandler<T>( Expression<Func<T>> propertyExpression, params IEnumerable<PropertyChangedEventHandler> handlers ) {
			if( propertyExpression == null ) throw new ArgumentNullException( nameof( propertyExpression ) );
			if( handlers == null ) throw new ArgumentNullException( nameof( handlers ) );
			if( propertyExpression.Body is not MemberExpression memberExpression ) {
				throw new NotSupportedException( "このメソッドでは ()=>プロパティ の形式のラムダ式以外許可されません" );
			}

			ThrowExceptionIfDisposed();
			_bag.RegisterHandler( memberExpression.Member.Name, handlers );
		}

		public void Add( PropertyChangedEventHandler handler ) => this.RegisterHandler( handler );
		public void Add( string propertyName, PropertyChangedEventHandler handler ) => this.RegisterHandler( propertyName, handler );
		public void Add( string propertyName, IEnumerable<PropertyChangedEventHandler> handlers ) => this.RegisterHandler( propertyName, handlers );
		public void Add<T>( Expression<Func<T>> propertyExpression, PropertyChangedEventHandler handler ) => this.RegisterHandler( propertyExpression, handler );
		public void Add<T>( Expression<Func<T>> propertyExpression, IEnumerable<PropertyChangedEventHandler> handlers ) => this.RegisterHandler( propertyExpression, handlers );
	}
}