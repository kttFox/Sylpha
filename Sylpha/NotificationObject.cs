using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Sylpha {
	/// <summary>
	/// 変更通知オブジェクトの基底クラス
	/// </summary>
	[Serializable]
	public class NotificationObject : INotifyPropertyChanged {
		/// <summary>
		/// プロパティ変更通知イベント
		/// </summary>
		[field: NonSerialized]
		public event PropertyChangedEventHandler? PropertyChanged;

		/// <summary>
		/// プロパティ変更通知イベントを発生させます。
		/// </summary>
		/// <param name="propertyName">プロパティ名</param>
		protected virtual void RaisePropertyChanged( [CallerMemberName] string propertyName = "" ) {
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
		}

		/// <summary>
		/// プロパティ変更通知イベントを発生させます。
		/// </summary>
		/// <typeparam name="T">プロパティの型</typeparam>
		/// <param name="propertyExpression">() => プロパティ 形式のラムダ式</param>
		/// <exception cref="ArgumentNullException"><paramref name="propertyExpression"/> が <c>null</c> の場合に発生します。</exception>
		/// <exception cref="NotSupportedException">() => プロパティ 以外の形式のラムダ式が指定された場合に発生します。</exception>
		protected virtual void RaisePropertyChanged<T>( Expression<Func<T>> propertyExpression ) {
			if( propertyExpression == null ) throw new ArgumentNullException( nameof( propertyExpression ) );
			if( propertyExpression.Body is not MemberExpression memberExpression ) throw new NotSupportedException( "このメソッドでは ()=>プロパティ の形式のラムダ式以外許可されません" );

			RaisePropertyChanged( memberExpression.Member.Name );
		}

		/// <summary>
		/// プロパティの値を設定します。値が変更された場合はプロパティ変更通知イベントを発生させます。
		/// </summary>
		/// <typeparam name="T">プロパティの型</typeparam>
		/// <param name="source">更新対象のフィールド（参照渡し）</param>
		/// <param name="value">設定する新しい値</param>
		/// <param name="propertyName">プロパティ名（<see cref="CallerMemberNameAttribute"/> により自動設定）</param>
		/// <returns>値が変更された場合は true、変更されなかった場合は false</returns>
		protected bool SetProperty<T>( ref T source, T value, [CallerMemberName] string propertyName = "" ) {
			if( EqualityComparer<T>.Default.Equals( source, value ) ) {
				return false;
			}

			source = value;
			RaisePropertyChanged( propertyName );
			return true;
		}
	}
}