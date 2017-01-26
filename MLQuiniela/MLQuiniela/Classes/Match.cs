using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLQuiniela.Classes
{
	/// <summary>
	/// Un Partido dado y los goles de uno y otro equipo
	/// </summary>
   public class Match
	{
		public string HomeTeam { get; set; }

		public string AwayTeam { get; set; }

		public int HomeTeamGoal { get; set; }

		public int AwayTeamGoal { get; set; }
    }
}
