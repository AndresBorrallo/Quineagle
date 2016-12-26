using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLQuiniela.Classes
{
    public class HistoricMatch
    {
        public string Div { get; set; }

        public string Date { get; set; }

        public string HomeTeam { get; set; }

        public string AwayTeam { get; set; }

        public string FTHG { get; set; }

        public string FTAG { get; set; }

        public string FTR { get; set; }

        public string HTHG { get; set; }

        public string HTAG { get; set; }

        public string HTR { get; set; }

        public string HS { get; set; }

        public string AS { get; set; }

        public string HST { get; set; }

        public string AST { get; set; }

        public string HF { get; set; }

        public string AF { get; set; }

        public string HC { get; set; }

        public string AC { get; set; }

        public string HY { get; set; }

        public string AY { get; set; }

        public string HR { get; set; }

        public string AR { get; set; }

        public HistoricMatch historicMatch(string div, string date, string homeTeam, string awayTeam, string fthg, string ftag, string ftr)
        {
            HistoricMatch hm = new HistoricMatch();
            hm.Div = div;
            hm.Date = date;
            hm.HomeTeam = homeTeam;
            hm.AwayTeam = awayTeam;
            hm.FTHG = fthg;
            hm.FTAG = ftag;
            hm.FTR = ftr;

            return hm;
        }
    }
}
