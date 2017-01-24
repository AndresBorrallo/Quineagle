using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;


namespace prueba_scraping
{
	public class Scrap
	{
		private const string PRIMERA = "";
		private const string SEGUNDA = "segundadivision";

		private Dictionary<int, List<Match>> History = new Dictionary<int, List<Match>>();

		public void LoadHistory( int YearFrom, int YearTo )
		{
			for( int y = YearFrom; y <= YearTo; y++ )
			{
				History.Add( y, new List<Match>() );

				var doc_primera = DownloadDocument( PRIMERA, y.ToString());

				var nodes = doc_primera.DocumentNode
					  .SelectNodes( "//div[@id=\"rt_Kreuztabelle\"]/table/tr/td" )
						.Skip( 1 );

				foreach( var nn in nodes.Skip( 1 ) )
				{
					nn.SelectNodes( "td[@class='Gegner']" );
					var fc = nn.ChildNodes;


				}


				//History[ y ].AddRange( GetMatchs(nodes) );
			}
		}

		/*private List<Match> GetMatchs( HtmlNode nodes )
		{
			List<Match> matchs = new List<Match>();

			nodes.SelectNodes( "//td[@class=\"Gegner\"]" )
			     .Select( a =>  matchs.Add( new Match()
				 {
					 LocalTeam = a.InnerHtml,
					 LocalTeamGoals = int.Parse( a.InnerText.Split( ':' )[ 0 ] ),
					 VisitingTeam = "..",
					 VisitingTeamGoals = int.Parse( a.InnerText.Split( ':' )[ 1 ] )
				 } ) );

			return matchs;

		}*/

		/// <summary>
		/// http://www.fussballdaten.de/spanien/2013/
		/// http://www.fussballdaten.de/spanien/segundadivision/2013/
		/// </summary>
		private HtmlDocument DownloadDocument( string liga ,string season )
		{
			var url = string.Format( $"http://www.fussballdaten.de/spanien/{liga}/{season}" );
			var html = new WebClient().DownloadString( url );

			var doc = new HtmlDocument();

			doc.LoadHtml( html );
			return doc;
		}

		public IEnumerable<Clasificacion> GetClassification( string liga, string season )
		{
			var doc = DownloadDocument( liga, season );

			return doc.DocumentNode
				      .SelectNodes( "//div[@id=\"rt_Tabelle\"]/table/tr")
					  .Skip( 1 )
				   	.Select( node => GetClassification( node ) );

			//var nodes = doc.DocumentNode.SelectNodes( "//div[@id='rt_Tabelle']/table/tr" ).Skip( 1 );
			//return r;
			//return null;
		}

		private Clasificacion GetClassification( HtmlNode node )
		{
			// El resultado tiene el formato goles_local - goles_visitante
			// Ejemplo: 1-3
			//var scoreParts = node.SelectSingleNode( "td[@class='resultado']" ).InnerText.Split( '-' );

			return new Clasificacion
			{
				Pos = int.Parse( node.SelectSingleNode( "td[contains(@class,'Platz')]" ).InnerText.Replace(".","" )),
				AgainstGoals = int.Parse( node.SelectSingleNode( "td[@class='Torverhaeltnis']" ).InnerText.Split( ':' )[ 1 ] ),
				FavourGoals = int.Parse( node.SelectSingleNode( "td[@class='Torverhaeltnis']" ).InnerText.Split( ':' )[ 0 ] ),
				MatchLost = int.Parse( node.SelectSingleNode( "td[@class='N']" ).InnerText ),
				MatchTied = int.Parse( node.SelectSingleNode( "td[@class='U']" ).InnerText ),
				MatchWins = int.Parse( node.SelectSingleNode( "td[@class='S']" ).InnerText ),
				Name = (node.SelectSingleNode( "td[@class='Verein']" ).InnerText).Replace("&#160;"," "),
				MatchPlayed = int.Parse( node.SelectSingleNode( "td[@class='Spiele']" ).InnerText ),
				GoalDiff = int.Parse( node.SelectSingleNode( "td[@class='Diff']" ).InnerHtml ),
				Points = int.Parse( node.SelectSingleNode( "td[@class='Punkte']" ).InnerText )

				/*
				 * <td class="posicion ascenso">3</td>
					<td class="equipo">Atlético</td>
					<td class="pj">6</td>
					<td class="pg">3</td>
					<td class="pe">3</td>
					<td class="pp">0</td>
					<td class="gf">12</td>
					<td class="gc">2</td>
					<td class="pts seleccionado">12</td>
				*/
			};
		}
	}
}
