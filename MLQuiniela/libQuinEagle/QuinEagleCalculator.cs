using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using libQuinEagle.Clasification;
using libQuinEagle.Fixtures;
using libQuinEagle.Fuzzy;
using libQuinEagle.Historic;
using libQuinEagle.Statistic;
using log4net;

namespace libQuinEagle
{
	/// <summary>
	/// Inicializa, configura y calcula probabilidades
	/// </summary>
	public class QuinEagleCalculator
	{
		private ILog Log { get; } = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public Configuration configuration = null;

		private HistoricMatchs _historicMatch = null;
		private ApiRequester _apiRequester = null;
		private FuzzyCalculator _fuzzy = null;

		private List<IStatistic> _statistics = new List<IStatistic>();

		public Fixture GetResult(Fixture fixture)
		{
			Fixture res = new Fixture() { HomeTeam = fixture.HomeTeam, AwayTeam = fixture.AwayTeam };
			List<Nomio> formula = new List<Nomio>();

			_statistics.ForEach(a => formula.Add(new Nomio() { Variable = a.GetStatistic(fixture), Weight = a.Weight }));

			res.Probability = formula.Sum(n => n.Variable * n.Weight);
			res.Result = _fuzzy.GetBet(res.Probability);

			return res;                    
		}

		/// <summary>
		/// Carga los elementos estadisticos
		/// </summary>
		private void _loadStatistic()
		{
			// Preparamos clases para hacer calculos
			IStatistic historical_st = new HistoricalStatistic()
			{
				historics = _historicMatch,
				Weight = configuration.HistoricWeight
			};

			_statistics.Add(historical_st);

			////////////

			IStatistic classification_st = new ClassificationStatistic()
			{
				req = _apiRequester,
				Weight = configuration.ClasificationWeight
			};

			_statistics.Add(classification_st);
		}

		public void Configure()
		{
			if (configuration == null)
				throw new ArgumentNullException();
			else
			{
				// Cargamos historicos
				Log.Info("Cargando historicos");
				_historicMatch = new HistoricMatchs();
				_historicMatch.LoadHistoric(configuration.Csv_URLs);

				// Cargamos clasificacion
				Log.Info("Cargando clasificacion");
				_apiRequester = new ApiRequester()
				{
					API_KEY = configuration.API_KEY,
					API_URL = configuration.API_URL,
					RequestHeader = configuration.RequestHeader,
					LeagueRequest = configuration.LeagueRequest
				};

				_loadStatistic();


				// Preparamos el Motor de logica difusa
				_fuzzy = new FuzzyCalculator() { MaxMultipleBets = configuration.FuzzyConf.MaxDoubles };
				_fuzzy.SetFuzzyValues(configuration.FuzzyConf.X1_1,
										configuration.FuzzyConf.X2_1,
										configuration.FuzzyConf.X1_X,
										configuration.FuzzyConf.X2_X,
										configuration.FuzzyConf.X3_X,
										configuration.FuzzyConf.X4_X,
										configuration.FuzzyConf.X1_2,
										configuration.FuzzyConf.X2_2);
			}	
		}
	}
}
