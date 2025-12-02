using static Tools;
using static Logger;
using static Histrogramm_maker;

public class Monte_Carlo
{
		List<DateTime> points_in_time { get; set; }
		List<ACTION> actions { get; set; }
		List<decimal> equity_curve { get; set; }


		public void simulate(int iterations, decimal start_capital)
		{

				List<decimal> original_bar_returns = return_foreach_bar(equity_curve);
				List<decimal> original_equity_curve = compound_returns(start_capital, original_bar_returns);
				double original_sharp = calculate_sharpe_ratio(original_bar_returns);


				List<double> simulated_sharps = new List<double>();

				for (int i = 0; i < iterations; i++)
				{
						List<List<decimal>> split_original_returns = break_into_batches(original_bar_returns, actions);
						List<List<decimal>> shuffled_returns = Fisher_Yates_Shuffle(split_original_returns);
						List<decimal> concatenated_data = concatenate_batches(shuffled_returns);
						simulated_sharps.Add(calculate_sharpe_ratio(concatenated_data));
						List<decimal> simulated_equity_curve = compound_returns(start_capital, concatenated_data);
						Console.WriteLine($"MONTE CARLO: {i} / {iterations} done");
				}

				Console.WriteLine("scattering data to grah...");

				Console.WriteLine("calculating p_value...");
				double counter = 0;
				foreach (double sharp in simulated_sharps)
				{
						if (sharp > original_sharp)
						{
								counter++;
						}
				}

				double p_value = Math.Round((counter + 1) / (simulated_sharps.Count() + 1), 3);
				log($"Original Sharp = {original_sharp}", "sharp");
				log($"P_VALUE = {p_value}", "sharp");
				make_sharp_distribution(simulated_sharps, original_sharp);
		}


		List<List<decimal>> break_into_batches(List<decimal> bar_returns, List<ACTION> states)
		{
				List<List<decimal>> batches = new List<List<decimal>>();
				List<decimal> batch = new List<decimal>();
				ACTION state = states.First();
				for (int i = 0; i < bar_returns.Count; i++)
				{
						if (states[i] != state && batch.Count() > 0)
						{
								batches.Add(batch);
								batch = new List<decimal>();
						}
						batch.Add(bar_returns[i]);
				}
				batches.Add(batch);
				return batches;
		}


		List<decimal> concatenate_batches(List<List<decimal>> batches)
		{
				List<decimal> full_batch = new List<decimal>();
				foreach (List<decimal> batch in batches)
				{
						foreach (decimal r in batch)
						{
								full_batch.Add(r);
						}
				}
				return full_batch;
		}

		public Monte_Carlo(List<DateTime> points_in_time, List<ACTION> actions, List<decimal> equity_curve)
		{
				this.points_in_time = points_in_time;
				this.actions = actions;
				this.equity_curve = equity_curve;
		}
}
