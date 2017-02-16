using System;
using System.Collections.Generic;

namespace libQuinEagle
{
	/// <summary>
	/// La configuracion de la Aplicacion, basada en el fichero Configuration.json
	/// </summary>
	public class Configuration
	{
		public float ClasificationWeight { get; set; }

		public float HistoricWeight { get; set; }

		public string QuinielaFixtureURL { get; set; }

		public string RequestHeader { get; set; }

		public string API_KEY { get; set; }

		//public string LeagueRequest { get; set; }
		public Dictionary<string, string> LeagueRequest { get; set; }

		public string API_URL { get; set; }

		public List<string> Csv_URLs { get; set; }
	}
}
