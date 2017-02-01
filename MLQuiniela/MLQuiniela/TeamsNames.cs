using System.Collections.Generic;
using System.Linq;

namespace MLQuiniela
{
	public static class Teams
	{
		public static Dictionary<string, List<string>> TeamsNames { get; set; }

		public static string GetKeyfromName( string name )
		{
			var key = TeamsNames.Where( x => x.Value.Contains( name ) ).Select( x => x.Key ).FirstOrDefault();

			if( !string.IsNullOrEmpty( key ) )
				name = key;

			return name;
		}
	}
}
