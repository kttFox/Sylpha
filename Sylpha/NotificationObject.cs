using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Sylpha {
	/// <summary>
	/// 変更通知オブジェクトの基底クラスです。
	/// </summary>
	[Serializable]
	public class NotificationObject : INotifyPropertyChanged {
		/// <summary>
		/// プロパティ変更通知イベントです。
		/// </summary>
		[field: NonSerialized]
		public event PropertyChangedEventHandler? PropertyChanged;

		/// <summary>
		/// プロパティ変更通知イベントを発生させます。
		/// </summary>
		/// <param name="source"></param>
		/// <param name="propertyExpression">() => プロパティ形式のラムダ式</param>
		/// <exception cref="NotSupportedException">() => プロパティ 以外の形式のラムダ式が指定されました。</exception>
		// ReSharper disable once UnusedParameter.Global
		protected virtual void RaisePropertyChanged<T>( Expression<Func<T>> propertyExpression ) {
			if( propertyExpression == null ) throw new ArgumentNullException( nameof( propertyExpression ) );
			if( propertyExpression.Body is not MemberExpression memberExpression ) throw new NotSupportedException( "このメソッドでは ()=>プロパティ の形式のラムダ式以外許可されません" );
			
			RaisePropertyChanged( memberExpression.Member.Name );
		}

		/// <summary>
		/// プロパティ変更通知イベントを発生させます
		/// </summary>
		/// <param name="propertyName">プロパティ名</param>
		protected virtual void RaisePropertyChanged( [CallerMemberName] string propertyName = "" ) {
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
		}

		/// <summary>
		/// プロパティの値を設定します。値が変更された場合はプロパティ変更通知イベントを発生させます。
		/// </summary>
		/// <typeparam name="T">プロパティの型</typeparam>
		/// <param name="source">元の値</param>
		/// <param name="value">新しい値</param>
		/// <param name="propertyName">プロパティ名</param>
		/// <returns>値の変更有無</returns>
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