using System;
using System.Net;
using Newtonsoft.Json;

namespace MLQuiniela.Classes
{
	/// <summary>
	/// Optiene una clasificacion dada usando la API de football-data.org
	/// </summary>
	public class ApiRequester
	{
		public string RequestHeader { get; set; }

		public string API_KEY { get; set; }

		public string LeagueRequest { get; set; }

		public string API_URL { get; set; }

		public LeagueTable GetLeagueTable( LeagueEnum league )
		{
			LeagueTable table = null;

			//string liga = league == LeagueEnum.PRIMERA ? PrimeraDivisionRequest : SegundaDivisionRequest;

			string request = $"{API_URL}{LeagueRequest}";

			request = request.Replace( "$LEAGUEID$", ( ( int )league ).ToString() );

			WebHeaderCollection headers = new WebHeaderCollection();
			headers.Add( RequestHeader, API_KEY );

			using( WebClient wc = new WebClient() { Headers = headers } )
			{
				var json = wc.DownloadString( request );
				table = JsonConvert.DeserializeObject<LeagueTable>( json );
			}

			return table;
		}

	}
}
