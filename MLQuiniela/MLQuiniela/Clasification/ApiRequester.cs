using System;
using System.Net;
using System.Reflection;
using log4net;
using Newtonsoft.Json;

namespace MLQuiniela.Clasification
{
	/// <summary>
	/// Optiene una clasificacion dada usando la API de football-data.org
	/// </summary>
	public class ApiRequester
	{
		private ILog Log { get; } = LogManager.GetLogger( MethodBase.GetCurrentMethod().DeclaringType );

		public string RequestHeader { get; set; }

		public string API_KEY { get; set; }

		public string LeagueRequest { get; set; }

		public string API_URL { get; set; }

		public LeagueTable GetLeagueTable( LeagueEnum league )
		{
			LeagueTable table = null;

			Log.Debug( $"Solicitando datos de la liga {EnumUtility.GetDescriptionFromEnumValue(league)}" );

			try
			{
				string request = $"{API_URL}{LeagueRequest}";

				request = request.Replace( "$LEAGUEID$", ( ( int )league ).ToString() );

				WebHeaderCollection headers = new WebHeaderCollection();
				headers.Add( RequestHeader, API_KEY );

				using( WebClient wc = new WebClient() { Headers = headers } )
				{
					Log.Debug( $"Request: {request}" );

					var json = wc.DownloadString( request );
					table = JsonConvert.DeserializeObject<LeagueTable>( json );
				}
			}
			catch( Exception e )
			{
				Log.Warn( $"Hubo un error al solicitar la clasificacion {EnumUtility.GetDescriptionFromEnumValue( league )}" );
				Log.Warn( e.Message );
			}

			return table;
		}

	}
}
