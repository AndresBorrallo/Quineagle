using System;
using System.Collections.Generic;

namespace Test_Csv
{
	public class Configuration
	{
		public string RequestHeader { get; set; }

		public string API_KEY { get; set; }

		public string PrimeraDivisionRequest { get; set; }

		public string SegundaDivisionRequest { get; set; }

		public string API_URL { get; set; }

		public List<string> Csv_URLs { get; set; }
	}
}
