//Copyright (c) 2025 Moritz Kolb
//Licensed for non-commercial use only. See LICENSE file for details.
using static Logger;
//tools for everywhere
public static class Tools
{
		public static decimal calculate_max_drawdown(List<Trade> trades)

		{
				List<decimal> worst_total_drawdowns = new List<decimal>();
				List<decimal> capital = new List<decimal>();

				foreach (Trade t in trades)
				{
						capital.Add(t.exit_capital);
				}

				for(int x = 1; x < capital.Count() -1; x++)
				{
					List<decimal> part_capital = capital.GetRange(0, x);
					decimal peak = part_capital.Max();
					int index_peak = part_capital.IndexOf(peak);
					decimal max_drawdown = 0;

					for (int i = index_peak; i < part_capital.Count(); i++)
					{

						decimal drawdown_t = ((part_capital[i] - peak) / peak) * 100;
						if (drawdown_t < max_drawdown)
						{
								max_drawdown = drawdown_t;
						}

					}

					worst_total_drawdowns.Add(max_drawdown);
				}

				var worst = worst_total_drawdowns.Min();
				
				return worst;

		}


		public static int calculate_longest_losing_streak(List<Trade> trades)
		{
				int longest = 0;
				int length = 0;
				foreach (Trade t in trades)
				{
						decimal trades_return = t.trades_return;
						if (trades_return < 0)
						{
								length++;
								if (length > longest)
								{
										longest = length;
								}
						}
						else
						{
								length = 0;
						}
				}

				return longest;
		}

		public static double calculate_expected_anual_return(List<Trade> trades)
		{
				decimal capital_start = trades.First().enter_capital;
				decimal capital_end = trades.Last().exit_capital;
				TimeSpan one_year_in_days = new TimeSpan(365, 0, 0, 0, 0);
				TimeSpan tradet_time = trades.Last().exit_time - trades.First().enter_time;
				double time_to_number = (one_year_in_days.TotalDays / tradet_time.TotalDays);
				decimal total = capital_end  / capital_start;
				double predicted_annual_returns = Math.Pow(Convert.ToDouble(total), time_to_number) - 1;

				predicted_annual_returns *= 100;
				predicted_annual_returns = Math.Round(predicted_annual_returns, 2);
				return predicted_annual_returns;
		}


		public static List<decimal> return_foreach_bar(List<decimal> equity)
		{
				List<decimal> bar_returns = new List<decimal>();
				for (int i = 0; i < equity.Count() - 1; i++)
				{
						decimal return_of_that_bar = (equity[i + 1] / equity[i]) - 1;
						bar_returns.Add(return_of_that_bar);
				}

				return bar_returns;
		}

		public static List<decimal> compound_returns(decimal capital, List<decimal> list_of_returns)
		{
				decimal start_capital = capital;
				List<decimal> compoundet_returns = new List<decimal>();
				foreach (decimal r in list_of_returns)
				{
						capital *= (1 + r);
						compoundet_returns.Add(capital);

				}
				return compoundet_returns;
		}

		public static double calculate_sharpe_ratio(List<decimal> bar_returns)
		{
				double risk_free_return = 0;
				double mean_return = Convert.ToDouble(bar_returns.Average());
				List<double> deviations = new List<double>();
				foreach (double r in bar_returns)
				{
						double deviation = r - mean_return;
						deviation *= deviation;
						deviations.Add(deviation);
				}
				double variance = deviations.Sum() / (deviations.Count() - 1);
				double standard_deviation = Math.Sqrt(variance);
				double sharpe_ration = (mean_return - risk_free_return) / standard_deviation;
				log($"sd{standard_deviation}","debug");
				log($"mean return{mean_return}","debug");

				return sharpe_ration;
		}


		static Random dice = new Random();
		public static List<List<decimal>> Fisher_Yates_Shuffle(List<List<decimal>> batch_list)
		{
				int n = batch_list.Count();
				for (int i = n - 1; i > 0; i--)
				{
						int random = dice.Next(i + 1);
						var tmp = batch_list[i];
						batch_list[i] = batch_list[random];
						batch_list[random] = tmp;

				}
				return batch_list;
		}


}



