using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;

namespace Sylpha {
	/// <summary>
	/// 変更通知オブジェクトの基底クラスです。
	/// </summary>
	[PublicAPI]
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
		[NotifyPropertyChangedInvocator]
		// ReSharper disable once UnusedParameter.Global
		protected virtual void RaisePropertyChanged<T>( ref T source, Expression<Func<T>> propertyExpression ) {
			if( propertyExpression == null ) throw new ArgumentNullException( nameof( propertyExpression ) );

			if( propertyExpression.Body is not MemberExpression ) throw new NotSupportedException( "このメソッドでは ()=>プロパティ の形式のラムダ式以外許可されません" );

			var memberExpression = (MemberExpression)propertyExpression.Body;
			RaisePropertyChanged( memberExpression.Member.Name );
		}

		/// <summary>
		/// プロパティ変更通知イベントを発生させます
		/// </summary>
		/// <param name="propertyName">プロパティ名</param>
		[NotifyPropertyChangedInvocator]
		protected virtual void RaisePropertyChanged( [CallerMemberName] string propertyName = "" ) {
			var threadSafeHandler = Interlocked.CompareExchange( ref PropertyChanged, null, null );
			threadSafeHandler?.Invoke( this, EventArgsFactory.GetPropertyChangedEventArgs( propertyName ) );
		}

		/// <summary>
		/// プロパティの値を設定します。値が変更された場合はプロパティ変更通知イベントを発生させます。
		/// </summary>
		/// <typeparam name="T">プロパティの型</typeparam>
		/// <param name="source">元の値</param>
		/// <param name="value">新しい値</param>
		/// <param name="propertyName">プロパティ名</param>
		/// <returns>値の変更有無</returns>
		[NotifyPropertyChangedInvocator]
		protected bool SetProperty<T>( ref T source, T value, [CallerMemberName] string propertyName = "" ) {
			//値が同じだったら何もしない
			if( EqualityComparer<T>.Default.Equals( source, value ) ) {
				return false;
			}

			source = value;
			RaisePropertyChanged( propertyName );

			return true;
		}

		
	}
}