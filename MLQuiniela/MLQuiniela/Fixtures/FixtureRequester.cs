using System.Linq;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;
using log4net;
using System.Reflection;
using System;
using MMLib;
using MMLib.Extensions;

namespace MLQuiniela.Fixtures
{
	/// <summary>
	/// Obtiene los emparejamientos de la quiniela usando webscraping de quinielistas.com
	/// </summary>
	public class FixtureRequester
	{
		private ILog Log { get; } = LogManager.GetLogger( MethodBase.GetCurrentMethod().DeclaringType );

		private List<Fixture> _Fixtures = new List<Fixture>();

		public string QuinielaFixtureURL { get; set; }

		public void LoadFixtures()
		{
			try
			{
				var html = new WebClient().DownloadString( QuinielaFixtureURL );
				var doc = new HtmlDocument();
				doc.LoadHtml( html );

				var nodes = doc.DocumentNode.SelectNodes( "//table[@class='general-table estimate-table upcoming-table']/tr" ).Skip( 3 );

				foreach( var node in nodes )
				{
					ConvertNoceToFixture( node );

				}
			}
			catch( Exception e )
			{
				Log.Warn( "Error al descargar los emparejamientos" );
				Log.Warn( e.Message );
			}

		}

		private void ConvertNoceToFixture( HtmlNode node )
		{
			var fx = new Fixture();

			var equipos = node.SelectSingleNode( "td[ @class='col-match cell text-match' ]/div" ).InnerHtml
							  .Replace( " - ", "-" ).Replace( "<br>", "-" )
							  .Split( '-' );

			var hometeam = equipos[ 0 ].RemoveDiacritics().ToUpper();
			var awayteam = equipos[ 1 ].RemoveDiacritics().ToUpper();

			fx.HomeTeam = Teams.GetKeyfromName( hometeam );
			fx.AwayTeam = Teams.GetKeyfromName( awayteam );

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
				/*
				 <div class="results-boxes clearfix"  style="float: right;">
                    <div class="r0"><span class="result-box ">&nbsp;</span></div>
                    <div class="r1"><span class="result-box ">&nbsp;</span></div>
                    <div class="r2"><span class="result-box active">&nbsp;</span></div>
                    <div class="rM"><span class="result-box ">&nbsp;</span></div>
                    <br>
                    <div class="r0" style="margin-top: 1px !important;"><span class="result-box ">&nbsp;</span></div>
                    <div class="r1" style="margin-top: 1px !important;"><span class="result-box ">&nbsp;</span></div>
                    <div class="r2" style="margin-top: 1px !important;"><span class="result-box active">&nbsp;</span></div>
                    <div class="rM" style="margin-top: 1px !important;"><span class="result-box ">&nbsp;</span></div>
                 </div>
				*/
				var hijos = div.ChildNodes;
				foreach( var h in hijos )
					if(h.FirstChild.Name == "span")
						Log.Info( h.FirstChild.Attributes[ 0 ].Value);

				var r0s = div.SelectNodes( "//div[@class='r0']/span" );
				var r1s = div.SelectNodes( "//div[@class='r1']/span" );
				var r2s = div.SelectNodes( "//div[@class='r2']/span" );
				var rms = div.SelectNodes( "//div[@class='rM']/span" );

				int local = -1;
				int visita = -1;

				// tratamos de convertir una apuesta del tipo (0,1,2,M) en (1,x,2)
				if( r0s[ 0 ].Attributes[ 0 ].Value == "result-box active" )
					local = 0;

				if( r0s[ 1 ].Attributes[ 0 ].Value == "result-box active" )
					visita = 0;

				if( r1s[ 0 ].Attributes[ 0 ].Value == "result-box active" )
					local = 1;

				if( r1s[ 1 ].Attributes[ 0 ].Value == "result-box active" )
					visita = 1;

				if( r2s[ 0 ].Attributes[ 0 ].Value == "result-box active" )
					local = 2;

				if( r2s[ 1 ].Attributes[ 0 ].Value == "result-box active" )
					visita = 2;

				if( rms[ 0 ].Attributes[ 0 ].Value == "result-box active" )
					local = 3;

				if( rms[ 1 ].Attributes[ 0 ].Value == "result-box active" )
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

		public List<Fixture> GetFixtures()
		{
			return _Fixtures;
		}

		public void PrintFixtures()
		{
			Log.Info( "Casa\tFuera\tResultado" );

			foreach( var a in _Fixtures )
			{
				Log.Info( $"{a.HomeTeam} - {a.AwayTeam}\t{EnumUtility.GetDescriptionFromEnumValue(a.Result)}" );
			}
		}
	}
}
