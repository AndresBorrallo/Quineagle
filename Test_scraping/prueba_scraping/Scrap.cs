using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;


namespace prueba_scraping
{
	public class Scrap
	{
		public Scrap()
		{
		}

		private HtmlDocument DownloadDocument( string season, int round )
		{
			var url = string.Format( $"http://www.marca.com/estadisticas/futbol/primera/{season}/jornada_{round}" );
			var html = new WebClient().DownloadString( url );

			html = html.Replace( "posicion ascenso", "posicion" );
			html = html.Replace( "posicion uefa", "posicion" );
			html = html.Replace( "posicion descenso", "posicion" );

			var doc = new HtmlDocument();

			doc.LoadHtml( html );
			return doc;
		}

		public IEnumerable<Clasificacion> GetMatches( string season, int round)
		{
			var doc = DownloadDocument(season, round);

			return doc.DocumentNode
				   .SelectNodes( "//table[@id='calsificacion_completa']/tbody/tr" )
				   .Select( node => GetClassification( node ) );
		}

		private Clasificacion GetClassification( HtmlNode node )
		{
			// El resultado tiene el formato goles_local - goles_visitante
			// Ejemplo: 1-3
			//var scoreParts = node.SelectSingleNode( "td[@class='resultado']" ).InnerText.Split( '-' );

			return new Clasificacion
			{
				Pos = int.Parse( node.SelectSingleNode( "td[@class='posicion']" ).InnerText ),
				AgainstGoals = int.Parse( node.SelectSingleNode( "td[@class='gc']" ).InnerText ),
				FavourGoals = int.Parse( node.SelectSingleNode( "td[@class='gf']" ).InnerText ),
				MatchLost = int.Parse( node.SelectSingleNode( "td[@class='pp']" ).InnerText ),
				MatchTied = int.Parse( node.SelectSingleNode( "td[@class='pe']" ).InnerText ),
				MatchWins = int.Parse( node.SelectSingleNode( "td[@class='pg']" ).InnerText ),
				Name = node.SelectSingleNode( "td[@class='equipo']" ).InnerText,
				MatchPlayed = int.Parse( node.SelectSingleNode( "td[@class='pj']" ).InnerText ),
				Points = int.Parse( node.SelectSingleNode( "td[@class='pts seleccionado']" ).InnerText )

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
