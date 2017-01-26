using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using MLQuiniela.Classes;

namespace MLQuiniela.Classes
{
	/// <summary>
	/// Carga historicos a partir de una lista de urls
	/// </summary>
	public class HistoricMatchs
	{
		private List<Match> _matchs = new List<Match>();

		public void LoadHistoric( List<string> urls )
		{
			_matchs.Clear();
			foreach( string url in urls )
			{
				Console.WriteLine( $"Descargando csv de {url}" );
				_loadHistoricFromUrl( url );
			}
		}

		private void _loadHistoricFromUrl( string url )
		{
			string fileList = _getCSV( url );
			IEnumerable<string> lines;

			lines = fileList.Split( '\n' ).Skip( 1 );

			foreach( string line in lines )
			{
				string[] values = line.Split( ',' );

				if( values.Length >= 6 &&
					!string.IsNullOrWhiteSpace( values[ 2 ] ) &&
				   	!string.IsNullOrWhiteSpace( values[ 3 ] ) &&
				  	!string.IsNullOrWhiteSpace( values[ 4 ] ) &&
				   	!string.IsNullOrWhiteSpace( values[ 5 ] ) )
				{
					Match m = new Match()
					{
						HomeTeam = values[ 2 ],
						AwayTeam = values[ 3 ],
						HomeTeamGoal = int.Parse( values[ 4 ] ),
						AwayTeamGoal = int.Parse( values[ 5 ] )
					};

					_matchs.Add( m );
				}
			}
		}

		private string _getCSV( string url )
		{
			HttpWebRequest req = ( HttpWebRequest )WebRequest.Create( url );
			HttpWebResponse resp = ( HttpWebResponse )req.GetResponse();

			StreamReader sr = new StreamReader( resp.GetResponseStream() );
			string results = sr.ReadToEnd();

			sr.Close();

			return results;
		}

		public MatchStatistic GetStatistic( string HomeTeam, string AwayTeam )
		{
			MatchStatistic ms = new MatchStatistic();
			IEnumerable<Match> matches = _matchs.Where( a => a.AwayTeam == AwayTeam && a.HomeTeam == HomeTeam );
			int n_matchs = matches.Count();

			ms.AwayTeam = AwayTeam;
			ms.HomeTeam = HomeTeam;
			ms.NMatchs = n_matchs;

			if( n_matchs > 0 )
			{
				int wins = matches.Where( a => a.HomeTeamGoal > a.AwayTeamGoal ).Count();
				int draws = matches.Where( a => a.HomeTeamGoal == a.AwayTeamGoal ).Count();
				int losts = matches.Where( a => a.HomeTeamGoal < a.AwayTeamGoal ).Count();

				ms.WinsPercent = ( float )wins/n_matchs * 100;
				ms.DrawsPercent = ( float )wins/n_matchs * 100;
				ms.LostPercent = ( float )losts/n_matchs  * 100;
			}
			else
			{
				Console.WriteLine( $"No se encontraron partidos para {HomeTeam} vs {AwayTeam}" );
			}

			return ms;
		}
	}
}
