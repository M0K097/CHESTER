//------->>> loads and manages strategies

//make an interface for all strategies
public interface IStrategy
{
		int warmup { get; set; }
		void init();
		void update(Candle c);
		int check();
		public void update_indicators(Candle c);
		public Dictionary<string, decimal> get_result_of(string name);
}

//define a parent class for all strategies
public class Strategy : IStrategy
{
		public int warmup { get; set; }
		public List<IIndicator> indicators = new List<IIndicator>();

		//use this to initiate your indicators
		public virtual void init()
		{

				Console.WriteLine("init function empty");
		}

		//gets updated every new candle
		//here your logic gets implemented
		public virtual void update(Candle c)
		{

				Console.WriteLine("update function empty");
		}

		//returns the value you want to buy
		// 0 = nothing, 20 = buy for 20% of capital, -100 = sell everything
		public virtual int check()
		{
				Console.WriteLine("check function is empty");

				return 0;
		}

		public void update_indicators(Candle c)
		{
				foreach (IIndicator I in indicators)
				{
						I.update(c);
				}

		}
		public Dictionary<string, decimal> get_result_of(string indicators_name)
		{
				Dictionary<string, IIndicator> book_of_indicators = indicators.ToDictionary(p => p.name);
				IIndicator selected = book_of_indicators[indicators_name];
				return selected.get_result();

		}
}

//instantiates the strategies
static public class Strategy_Manager
{

		//select a installed strategy over CLI_UI
		static public IStrategy select_strategy()
		{

				//instantiate all strategies
				//-------------------------------->>
				IStrategy NinjaTurtle_v1 = new NinjaTurtle();
				IStrategy TrendFollower = new SimpleTrendFollowingSystem();
				IStrategy MACDMOMENTUM = new MACDMOMENTUM();
				IStrategy test = new test_strategy();


				List<IStrategy> list_of_all_strategies = new List<IStrategy> { TrendFollower, MACDMOMENTUM, test, NinjaTurtle_v1 };
				//-------------------------------->>

				//select and return a new instance of a strategy over CLI_UI
				List<string?> strategy_names = new List<string?>();

				foreach (IStrategy s in list_of_all_strategies)
				{
						if (s != null)
						{

								strategy_names.Add(s.ToString());

						}
						else
						{
								strategy_names.Add("Error! => no strategies installed");

						}
				}

				//select a strategy over cli_menu
				string selected_name = CLU_UI.run(strategy_names!, "_____SELECT A STRATEGY______");

				IStrategy selected = new Strategy();

				//load the selected strategy
				foreach (IStrategy s in list_of_all_strategies)
				{
						if (s.ToString() == selected_name)
						{
								selected = s;
						}
				}

				//return the selected strategy interface
				return selected!;
		}
}

