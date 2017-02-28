using System;
using System.Collections.Generic;

namespace libQuinEagle
{
	public class FuzzyConf
	{
		public float X1_1 { get; set; }

		public float X2_1 { get; set; }

		public float X1_X { get; set; }

		public float X2_X { get; set; }

		public float X3_X { get; set; }

		public float X4_X { get; set; }

		public float X1_2 { get; set; }

		public float X2_2 { get; set; }

		public int MaxDoubles { get; set; }
	}
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

		public List<Tuple<string,string,bool>> Csv_URLs { get; set; }

		public FuzzyConf FuzzyConf { get; set; }
	}
}
