//Copyright (c) 2025 Moritz Kolb
//Licensed for non-commercial use only. See LICENSE file for details.

using static Logger;
using static Tools;
//statistical analysis
public enum ACTION
{
		nothing,
		buy,
		sell,
}
public class Analyzer
{
		public List<DateTime> time = new List<DateTime>();
		public List<Decimal> price = new List<decimal>();
		public List<Decimal> capital = new List<decimal>();
		public List<ACTION> actions = new List<ACTION>();

		public void update(DateTime time, decimal price, decimal capital, ACTION action)
		{
				this.time.Add(time);
				this.price.Add(price);
				this.capital.Add(capital);
				this.actions.Add(action);
		}

		public void inspect_trades(List<Trade> trades)
		{
				Dictionary<string, Trade> list_trades = new Dictionary<string, Trade>();
				List<string> trade_names = new List<string>();
				int index = 1;
				if (trades.Count() > 0)
				{
						foreach (Trade t in trades)
						{
								string name = $"Nr.{index,-5}{t.enter_time,-30}{t.exit_time,-30}{"=>",-10} {Math.Round(t.trades_return, 2),0}%";

								list_trades.Add(name, t);
								trade_names.Add(name);
								index++;
						}
						while (true)
						{
								string selected_trade = CLU_UI.run(trade_names, "TRADES");
								if (selected_trade == "return")
								{
										break;
								}
								list_trades[selected_trade].show_trade();
								Console.ReadLine();
						}
				}
				else
				{
						log("PROBLEM: no trades to inspect", "METRICS");
				}
		}


		public void make_metrics(List<Trade> trades)
		{
				List<Trade> list_trades = trades;
				List<decimal> profit_trades = new List<decimal>();
				List<decimal> loosing_trades = new List<decimal>();
				foreach (Trade t in list_trades)
				{
						if (t.trades_return > 0)
						{
								profit_trades.Add(t.trades_return);
						}
						if (t.trades_return < 0)
						{
								loosing_trades.Add(t.trades_return);
						}

				}
				if (loosing_trades.Count() > 0 && profit_trades.Count() > 0)
				{
						decimal max_profit_trade = profit_trades.Max();
						decimal max_loss_trade = loosing_trades.Min();
						decimal average_loss = loosing_trades.Average();
						decimal average_profit = profit_trades.Average();
						decimal STRATEGY_PERFORMANCE = (((trades.Last().exit_capital - trades.First().enter_capital) / trades.First().enter_capital) * 100);
						decimal MARKET_PERFORMANCE = (((price.Last() - price.First()) / price.First()) * 100);
						decimal profit_factor = profit_trades.Sum() / Math.Abs(loosing_trades.Sum());
						decimal start_capital = trades.First().enter_capital;
						decimal end_capital = trades.Last().exit_capital;
						double hitrate = ((double)profit_trades.Count() / list_trades.Count());
						int amount_trades = trades.Count();
						List<decimal> bar_returns = return_foreach_bar(capital);
						double sharp = calculate_sharpe_ratio(bar_returns);
						decimal max_drawdown = calculate_max_drawdown(trades);
						decimal longest_losing_streak = calculate_longest_losing_streak(trades);
						DateTime start_time = trades.First().enter_time;
						DateTime end_time = trades.Last().exit_time;
						TimeSpan timespan = end_time - start_time;
						double predicted_annual = calculate_expected_anual_return(trades);
						double expectancy = (hitrate * (double)average_profit) - ((1 - hitrate) * (double)average_loss);


						string metrics = ($"\n_______FULL METRIC_______________________\n" +
							$"{"START_TIME",20} {":",-10}{start_time,-5}\n" +
							$"{"END_TIME",20} {":",-10}{end_time,-5}\n" +
							$"{"TIMESPAN",20} {":",-10}{timespan,-5}\n" +
							$"{"START_CAPITAL",20} {":",-10}{start_capital,-5:F2}\n" +
							$"{"END_CAPITAL",20} {":",-10}{end_capital,-5:F2}\n" +
							$"{"CAPITAL GROWTH",20} {":",-10}{STRATEGY_PERFORMANCE,-5:F2}%\n" +
							$"{"MARKET",20} {":",-10}{MARKET_PERFORMANCE,-5:F2}%\n" +
							$"{"AMOUNT_OF_TRADES",20} {":",-10}{amount_trades,-5}\n" +
							$"{"AVERAGE_PROFIT",20} {":",-10}{average_profit,-5:F2}%\n" +
							$"{"AVERAGE_LOSS",20} {":",-10}{average_loss,-5:F2}%\n" +
							$"{"MAX_PROFIT_TRADE",20} {":",-10}{max_profit_trade,-5:F2}%\n" +
							$"{"MAX_LOSS_TRADE",20} {":",-10}{max_loss_trade,-5:F2}%\n" +
							$"{"PROFIT_FACTOR",20} {":",-10}{profit_factor,-5:F2}\n" +
							$"{"HITRATE",20} {":",-10}{hitrate * 100,-5:F2}%\n" +
							$"{"SHARP_RATIO",20} {":",-10}{sharp,-5:F4}\n" +
							$"{"MAX_DRAWDOWN",20} {":",-10}{max_drawdown,-5:F2}\n" +
							$"{"LONGEST_LOOSINGSTREAK",20} {":",-10}{longest_losing_streak,-5}\n" +
							$"{"EXPECTANCY",20} {":",-10}{expectancy,-5:F}\n" +
							$"{"PREDICTED_ANNUAL",20} {":",-10}{predicted_annual,-5}%");

						log(metrics, "METRICS");
						Console.WriteLine(metrics);
						Console.WriteLine("Press Enter...");
						Console.ReadLine();
				}
				else
				{
						log("PROBLEM: no data for METRICS", "METRICS");
				}

		}

		public List<Trade> reproduce_trades()
		{
				bool holding = false;
				Trade trade = new Trade(0);
				List<Trade> trades = new List<Trade>();
				int index = 0;
				for (int x = 0; x < actions.Count() - 1; x++)
				{

						if (actions[x] == ACTION.buy && !holding)
						{
								index++;
								trade = new Trade(index);
								trade.set_entry(time[x], price[x], capital[x]);
								holding = true;
						}
						else if (actions[x] == ACTION.sell && holding)
						{
								trade.set_exit(time[x], price[x], capital[x]);
								trade.calculate_return();
								trades.Add(trade);
								holding = false;
						}
				}
				return trades;
		}
		
		
}
