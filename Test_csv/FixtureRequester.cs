using System.Linq;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;

namespace Test_Csv
{
	public class FixtureRequester
	{
		private List<Fixture> _Fixtures = new List<Fixture>();

		public string QuinielaFixtureURL { get; set; }

		public void LoadFixtures()
		{
			var html = new WebClient().DownloadString( QuinielaFixtureURL );
			var doc = new HtmlDocument();
			doc.LoadHtml( html );

			var nodes = doc.DocumentNode.SelectNodes( "//table[@class='general-table estimate-table upcoming-table']/tr" ).Skip(3);

			foreach( var node in nodes )
			{
				Fixture fx = new Fixture();

				var equipos = node.SelectSingleNode( "td[ @class='col-match cell text-match' ]/div" ).InnerHtml
								  .Replace( " - ", "-" ).Replace( "<br>", "-" )
				                  .Split( '-' );

				fx.HomeTeam = equipos[ 0 ];
				fx.AwayTeam = equipos[ 1 ];

				// Miramos si es un emparejamiento clasico (1,x,2) o de los nuevos (0,1,2,M)
				var div = node.SelectSingleNode( "td[ @class='cell results results-centered' ]/div" );

				if( div.SelectSingleNode( "div[@class='rx']" ) != null )
				{
					var r1 = div.SelectSingleNode( "div[@class='r1']" );
					var rx = div.SelectSingleNode( "div[@class='rx']" );
					var r2 = div.SelectSingleNode( "div[@class='r2']" );

					if( r1.ChildNodes[ 0 ].Attributes[ 0 ].Value == "result-box active" )
						fx.Result = QuinielaResult.ONE;
					else if( r2.ChildNodes[ 0 ].Attributes[ 0 ].Value == "result-box active" )
						fx.Result = QuinielaResult.TWO;
					else if( rx.ChildNodes[ 0 ].Attributes[ 0 ].Value == "result-box active" )
						fx.Result = QuinielaResult.X;
					else
						fx.Result = QuinielaResult.VOID;
				}
				else
				{
					var r0s = div.SelectNodes( "//div[@class='r0']" );
					var r1s = div.SelectNodes( "//div[@class='r1']" );
					var r2s = div.SelectNodes( "//div[@class='r2']" );
					var rms = div.SelectNodes( "//div[@class='rM']" );

					int local = -1;
					int visita = -1;

					// tratamos de convertir una apuesta del tipo (0,1,2,M) en (1,x,2)
					if( r0s[ 0 ].ChildNodes[ 0 ].Attributes[ 0 ].Value == "result-box active" )
						local = 0;
					if( r0s[ 1 ].ChildNodes[ 0 ].Attributes[ 0 ].Value == "result-box active" )
						visita = 0;
					if( r1s[ 0 ].ChildNodes[ 0 ].Attributes[ 0 ].Value == "result-box active" )
						local = 1;
					if( r1s[ 1 ].ChildNodes[ 0 ].Attributes[ 0 ].Value == "result-box active" )
						visita = 1;
					if( r2s[ 0 ].ChildNodes[ 0 ].Attributes[ 0 ].Value == "result-box active" )
						local = 2;
					if( r2s[ 1 ].ChildNodes[ 0 ].Attributes[ 0 ].Value == "result-box active" )
						visita = 2;
					if( rms[ 0 ].ChildNodes[ 0 ].Attributes[ 0 ].Value == "result-box active" )
						local = 3;
					if( rms[ 1 ].ChildNodes[ 0 ].Attributes[ 0 ].Value == "result-box active" )
						visita = 3;

					QuinielaResult r = QuinielaResult.VOID;

					if( local > -1 && visita > -1 )
					{
						if( local == visita )
							r = QuinielaResult.X;
						else if( local > visita )
							r = QuinielaResult.ONE;
						else if( local < visita )
							r = QuinielaResult.TWO;
					}

					fx.Result = r;
				}

				_Fixtures.Add( fx );
			}

		}

		public List<Fixture> GetFixtures()
		{
			return _Fixtures;
		}
	}
}
