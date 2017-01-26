using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLQuiniela.Classes
{
	/// <summary>
	/// Relaciona dos equipos y su resultado
	/// </summary>
    public class Fixture
	{
		public string HomeTeam { get; set; }

		public string AwayTeam { get; set; }

		public QuinielaResult Result { get; set; }
	}
}
