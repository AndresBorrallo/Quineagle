using System;
using System.Collections.Generic;

namespace MLQuiniela
{
	/// <summary>
	/// La configuracion de la Aplicacion, basada en el fichero Configuration.json
	/// </summary>
	public class Configuration
	{
		public string QuinielaFixtureURL { get; set; }

		public string RequestHeader { get; set; }

		public string API_KEY { get; set; }

		public string LeagueRequest { get; set; }

		public string API_URL { get; set; }

		public List<string> Csv_URLs { get; set; }
	}
}
