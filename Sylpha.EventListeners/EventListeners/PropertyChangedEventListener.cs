using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Sylpha.EventListeners {
	/// <summary>
	/// <see cref="INotifyPropertyChanged.PropertyChanged"/> を受信するためのイベントリスナーです。
	/// </summary>
	public sealed class PropertyChangedEventListener : EventListener<PropertyChangedEventHandler>, IEnumerable<KeyValuePair<string, List<PropertyChangedEventHandler>>>, IDisposable {
		private readonly WeakReference<INotifyPropertyChanged> _source;
		private readonly Dictionary<string, List<PropertyChangedEventHandler>> _handlerDictionary = [];

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">INotifyPropertyChangedオブジェクト</param>
		public PropertyChangedEventListener( INotifyPropertyChanged source ) {
			if( source == null ) throw new ArgumentNullException( nameof( source ) );
			_source = new( source );

			Initialize(
				h => source.PropertyChanged += h,
				h => source.PropertyChanged -= h,
				( sender, e ) => ExecuteHandler( e )
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
		/// このリスナーインスタンスに新たなハンドラを追加します。
		/// </summary>
		/// <param name="handler">PropertyChangedイベントハンドラ</param>
		public void RegisterHandler( PropertyChangedEventHandler handler ) {
			if( handler == null ) throw new ArgumentNullException( nameof( handler ) );

			ThrowExceptionIfDisposed();
			RegisterHandler( string.Empty, handler );
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

			if( !_handlerDictionary.TryGetValue( propertyName, out var list ) ) {
				_handlerDictionary[propertyName] = list = [];
			}
			list.AddRange( handlers );
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
			RegisterHandler( memberExpression.Member.Name, handlers );
		}

		public void Add( PropertyChangedEventHandler handler ) => this.RegisterHandler( handler );
		public void Add( string propertyName, PropertyChangedEventHandler handler ) => this.RegisterHandler( propertyName, handler );
		public void Add( string propertyName, IEnumerable<PropertyChangedEventHandler> handlers ) => this.RegisterHandler( propertyName, handlers );
		public void Add<T>( Expression<Func<T>> propertyExpression, PropertyChangedEventHandler handler ) => this.RegisterHandler( propertyExpression, handler );
		public void Add<T>( Expression<Func<T>> propertyExpression, IEnumerable<PropertyChangedEventHandler> handlers ) => this.RegisterHandler( propertyExpression, handlers );
	}
}