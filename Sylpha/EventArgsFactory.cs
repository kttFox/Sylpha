using System.Collections.Concurrent;
using System.ComponentModel;

namespace Sylpha {
	internal static class EventArgsFactory {
		private static readonly ConcurrentDictionary<string, PropertyChangedEventArgs> PropertyChangedEventArgsDictionary = new();

		public static PropertyChangedEventArgs GetPropertyChangedEventArgs( string? propertyName ) 
			=> PropertyChangedEventArgsDictionary.GetOrAdd( propertyName ?? string.Empty, name => new PropertyChangedEventArgs( name ) );
	}
}