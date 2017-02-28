using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using log4net;
using MMLib.Extensions;

namespace libQuinEagle.Historic
{
	/// <summary>
	/// Carga historicos a partir de una lista de urls
	/// </summary>
	public class HistoricMatchs
	{
		private const string FOLDERPATH = @"csvs";

		private ILog Log { get; } = LogManager.GetLogger( MethodBase.GetCurrentMethod().DeclaringType );

		private List<Match> _matchs = new List<Match>();

		public void LoadHistoric( List<Tuple<string,string,bool>> files )
		{
			_matchs.Clear();

			// Comprobar si existe el directorio con los ficheros csv
			if( !Directory.Exists( FOLDERPATH ) )
			{
				Directory.CreateDirectory( FOLDERPATH );
			}

			// Por cada fichero, comprobar si existe y si no bajarlo
			// algunos ficheros hay que forzar bajarlos existan o no
			foreach(var t in files)
			{
				if( !File.Exists( $"{FOLDERPATH}/{t.Item1}" ) || t.Item3 )
				{
					_download_csv( t.Item1, t.Item2);
				}
				_loadHistoricFromFile( t.Item1 );
			}

			Log.Info( $"Historical Loaded with {_matchs.Count} matchs" );
		}

		private void _loadHistoricFromFile( string file_name )
		{
			try
			{

				string fileList = _getCSV( file_name );
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
							HomeTeam = Teams.GetKeyfromName(values[ 2 ].RemoveDiacritics().ToUpper()),
							AwayTeam = Teams.GetKeyfromName(values[ 3 ].RemoveDiacritics().ToUpper()),
							HomeTeamGoal = int.Parse( values[ 4 ] ),
							AwayTeamGoal = int.Parse( values[ 5 ] )
						};

						_matchs.Add( m );
					}
				}
			}
			catch( Exception e )
			{
				Log.Warn( "Excepcion descargando ficheros historicos" );
				Log.Warn( e.Message );
			}
		}

		private string _getCSV( string file_name )
		{
			return File.ReadAllText( $"{FOLDERPATH}/{file_name}" );
		}

		private void _download_csv( string file_name, string url )
		{
			Log.Debug( $"Descargando fichero {file_name} de {url}" );

			HttpWebRequest req = ( HttpWebRequest )WebRequest.Create( url );
			HttpWebResponse resp = ( HttpWebResponse )req.GetResponse();

			StreamReader sr = new StreamReader( resp.GetResponseStream() );
			string csv = sr.ReadToEnd();

			File.WriteAllText( $"{FOLDERPATH}/{file_name}", csv );

			sr.Close();
		}

		public MatchStatistic GetStatistic( string HomeTeam, string AwayTeam )
		{
			MatchStatistic ms = new MatchStatistic();
			IEnumerable<Match> matches = _matchs.Where( a => a.AwayTeam == AwayTeam && a.HomeTeam == HomeTeam );
			int n_matchs = matches.Count();

			ms.AwayTeam = AwayTeam;
			ms.HomeTeam = HomeTeam;
			ms.NMatchs = n_matchs;
            ms.NPoints = n_matchs * 3;

			if( n_matchs > 0 )
			{
				int wins = matches.Where( a => a.HomeTeamGoal > a.AwayTeamGoal ).Count();
				int draws = matches.Where( a => a.HomeTeamGoal == a.AwayTeamGoal ).Count();
				int losts = matches.Where( a => a.HomeTeamGoal < a.AwayTeamGoal ).Count();

				ms.WinsPercent = ( float )wins/n_matchs * 100;
				ms.DrawsPercent = ( float )draws/n_matchs * 100;
				ms.LostPercent = ( float )losts/n_matchs  * 100;

                ms.HomeNPoints = wins * 3 + draws;
                ms.AwayNPoints = losts * 3 + draws;

                ms.GoalsInFavour = matches.Sum( x => x.HomeTeamGoal );
				ms.GoalsAgainst = matches.Sum( x => x.AwayTeamGoal );
			}
			else
			{
				Log.Warn( $"No se encontraron partidos para {HomeTeam} vs {AwayTeam}" );
			}

			return ms;
		}
	}
}
