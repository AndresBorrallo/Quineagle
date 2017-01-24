using System;
using System.Collections.Generic;
using System.Linq;

namespace prueba_scraping
{
	class MainClass
	{
		public static void Main( string[] args )
		{
			Console.WriteLine( "Hello World!" );

			Scrap scrap = new Scrap();

			IEnumerable<Clasificacion>clasificacion = scrap.GetClassification( "", "2017");

			/*
			td class="posicion ascenso">3</td>
			<td class="equipo">Atlético</td>
			<td class="pj">6</td>
			<td class="pg">3</td>
			<td class="pe">3</td>
			<td class="pp">0</td>
			<td class="gf">12</td>
			<td class="gc">2</td>
			<td class="pts seleccionado">12</td>
			*/

			Console.WriteLine( "Pos\tEquipo\t\t\tPJ\tPG\tPE\tPP\tGF\tGC\tPTS" );

			foreach( Clasificacion c in clasificacion )
			{
				Console.WriteLine( $"{c.Pos}\t{c.Name}\t{c.MatchPlayed}\t{c.MatchWins}\t{c.MatchTied}\t{c.MatchLost}\t{c.FavourGoals}\t{c.AgainstGoals}\t{c.Points}" );
			}

			scrap.LoadHistory( 1993, 1995 );

			Console.ReadKey();
		}
	}
}
