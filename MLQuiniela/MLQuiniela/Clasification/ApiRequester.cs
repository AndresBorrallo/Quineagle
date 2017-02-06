using System;
using System.Net;
using System.Linq;
using System.Reflection;
using log4net;
using MMLib.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

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

		private Dictionary<LeagueEnum, LeagueTable> _leagues = new Dictionary<LeagueEnum, LeagueTable>();
		//private LeagueTable _primeraDivision = null;

		//private LeagueTable _segundaDivision = null;


		public void DownloadLeague( LeagueEnum league )
		{
			LeagueTable table = null;

			Log.Debug( $"Solicitando datos de la liga {EnumUtility.GetDescriptionFromEnumValue(league)}" );

			try
			{
				string request = $"{API_URL}{LeagueRequest}";

				request = request.Replace( "$LEAGUEID$", ( ( int )league ).ToString() );

				WebHeaderCollection headers = new WebHeaderCollection();
				headers.Add( RequestHeader, API_KEY );

                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                wc.Headers = headers;

                Log.Debug( $"Request: {request}" );
            	var json = wc.DownloadString( request );
				table = JsonConvert.DeserializeObject<LeagueTable>( json );

				// convierto los nombres a mayusculas y sin acentos
				table.standing.Select( a => { a.teamName = a.teamName.RemoveDiacritics().ToUpper(); return a; } ).ToList();
				// Por cada nombre en la tabla, lo busco en la TeamsNames y lo sustituyo si lo encuentra
				foreach( var team in table.standing )
				{
					team.teamName = Teams.GetKeyfromName( team.teamName );
				}
			}
			catch( Exception e )
			{
				Log.Warn( $"Hubo un error al solicitar la clasificacion {EnumUtility.GetDescriptionFromEnumValue( league )}" );
				Log.Warn( e.Message );
			}

			_leagues[ league ] = table;
		}

		public LeagueTable GetLeague( LeagueEnum league )
		{
			LeagueTable tabla = null;

			if( !_leagues.TryGetValue( league, out tabla ) )
			{
				Log.Warn( $"No existe la liga {EnumUtility.GetDescriptionFromEnumValue(league)}" );
			}

			return tabla;
		}

		public void PrintLeague( LeagueEnum league )
		{
			LeagueTable liga = null;

			if( _leagues.TryGetValue( league, out liga ))
			{
				Log.Info( $"Jornada numero {liga.matchday}" );
				Log.Info( $"POS - NOMBRE - PUNTOS - JUGADOS - GANADOS - PERDIDOS - EMPATADOS - G.FAVOR - G.CONTRA" );

				foreach( var c in liga.standing )
				{
					Log.Info( $"{c.position} - {c.teamName} - {c.points} - {c.playedGames} - {c.wins} - {c.losses} - {c.draws} - {c.goals} - {c.goalsAgainst}" );
				}
			}
		}

	}
}
