//Copyright (c) 2025 Moritz Kolb
//Licensed for non-commercial use only. See LICENSE file for details.
using static Logger;

//------------->>[SIMULATOR]<<------------>o>o>>

public class Simulator
{
		//market parameters
		public decimal quote_asset { get; set; }
		public decimal base_asset = 0;
		decimal slippage { get; set; }
		decimal fees { get; set; }
		decimal slipped_price = 0;
		public decimal total_capital;
		Candle base_asset_candle = new Candle();
		public Analyzer statistics = new Analyzer();

		//load the strategy
		IStrategy selected_strategy;

		//safe capital and value at start
		decimal start_capital;
		decimal base_asset_value_start = -1;

		//safe amount of trades
		public int trades = 0;
		int counter = 0;
		public int warmup = 0;
		bool initiated = false;
		public bool running_in_Chester = false;

		//safe data
		public List<Candle> data = new List<Candle>();
		public List<decimal> equity_curve = new List<decimal>();
		List<DateTime> time_data = new List<DateTime>();

		//safe trade points for plotting
		Dictionary<DateTime, decimal> buy_points = new Dictionary<DateTime, decimal> { };
		Dictionary<DateTime, decimal> sell_points = new Dictionary<DateTime, decimal> { };

		void initiate(Candle c)
		{
				log($"START SIMULATION", "SIMULATOR");
				log($"Start-Balance: Quote Asset[{quote_asset}]  Base Asset[{base_asset}]", "SIMULATOR");
				base_asset_value_start = c.close;
				selected_strategy.init();
				warmup = selected_strategy.warmup;
				log($"Warmup is set to {warmup}", "SIMULATOR");
				initiated = true;
		}

		//run strategy
		public void update(Candle c)
		{
				if (!initiated)
				{
						initiate(c);
				}
				base_asset_candle = c;
				data.Add(c);
				time_data.Add(c.close_time);
				selected_strategy.update_indicators(c);
				selected_strategy.update(c);

				
				int result = 0;
				if (running_in_Chester)
				{
					counter++;
				}
				if (counter >= warmup)
				{
					result = selected_strategy.check();
				}

				ACTION action = ACTION.nothing;
				if (result != 0)
				{
						trade(result);
						if (result > 0)
						{
								action = ACTION.buy;
						}
						else
						{
								action = ACTION.sell;
						}
				}

				if(!running_in_Chester)
				{
					counter++;
				}

				total_capital = quote_asset + base_asset * base_asset_candle.close;
				statistics.update(c.close_time, c.close, total_capital, action);
				equity_curve.Add(total_capital);

		}

		public void show_statistics()
		{
				List<Trade> trades = statistics.reproduce_trades();
				statistics.make_metrics(trades);
				graph(data);
				statistics.inspect_trades(trades);

		}

		public void do_monte_carlo_test()
		{
				var time = statistics.time;
				var actions = statistics.actions;
				var equity = statistics.capital;
				Monte_Carlo monte_carlo_test = new Monte_Carlo(time, actions, equity);
				monte_carlo_test.simulate(2000, start_capital);

		}


		//simulates a trade
		void trade(decimal percentage)
		{
				decimal amount = 0;
				decimal amount_base_asset = 0;

				if (percentage > 100 || percentage < -100)
				{
						Console.WriteLine($"Warning: [{percentage}]= wrong input, percentage cant be outside of -100% to 100%");
				}
				else if (percentage > 0)
				{
						amount = (quote_asset / 100) * percentage;
						decimal c = base_asset_candle.close;
						slipped_price = c + (c * (slippage / 100));
						amount_base_asset = (amount / slipped_price);
						buy_points.Add(base_asset_candle.close_time, base_asset_candle.close);
				}
				else if (percentage < 0)
				{
						amount_base_asset = (base_asset / 100) * percentage;
						decimal c = base_asset_candle.close;
						slipped_price = c - (c * (slippage / 100));
						amount = (amount_base_asset * slipped_price);
						sell_points.Add(base_asset_candle.close_time, base_asset_candle.close);
				}

				simulate_capital(amount, amount_base_asset);
		}

		public void simulate_capital(decimal amount, decimal amount_base_asset)
		{

				if (amount == 0)
				{
						log("Warning! can't execute the trade because not liquid", "SIMULATOR");
				}
				else if (amount > 0)
				{
						trades++;
						//simulate effect on account balance	
						quote_asset -= amount;
						base_asset += amount_base_asset;
						//apply fee
						decimal fee = amount_base_asset * (fees / 100);
						base_asset -= fee;
						//print result 
						log($"TRADE SIMULATED\n" +
								$"Time: {base_asset_candle.close_time}\n" +
								$"Price: {base_asset_candle.close}\n" +
								$"Slipped_PRICE: {slipped_price}\n" +
								$"BUY {amount_base_asset} of Base_Asset for {amount} of Quote_Asset\n" + $"Fee:{fee}\n" +
								$"[{quote_asset}] ----->> [{base_asset}]\n" +
								$"------------------------------------------------------------------>>>", "SIMULATOR");


				}
				else if (amount < 0)
				{
						trades++;
						quote_asset -= amount;
						base_asset += amount_base_asset;
						//apply fee
						decimal fee = (amount * (fees / 100));
						quote_asset += fee;
						//print results
						log($"TRADE SIMULATED\n" +
								$"Time: {base_asset_candle.close_time}\n" +
								$"Price: {base_asset_candle.close}\n" +
								$"Slipped_PRICE: {slipped_price}\n" +
								$"BUY {-amount_base_asset} of Base_Asset for {-amount} of Quote_Asset\n" +
								$"Fee:{-fee}\n" +
								$"[{quote_asset}] <<----- [{base_asset}]\n" +
								$"------------------------------------------------------------------>>>", "SIMULATOR");

				}

		}

		//calculate profit
		void graph(List<Candle> data)
		{
				Plotter graph = new Plotter("visualization/graph_backtest.png");

				graph.scatter_data(time_data, equity_curve);
				graph.show_candles(data);
				graph.mark_points(buy_points, "buys");
				graph.mark_points(sell_points, "sells");
				graph.safe_plot();
		}

		//constructor
		public Simulator(decimal capital_amount, decimal fees_percent, decimal slippage_amount, IStrategy strat)
		{
				quote_asset = capital_amount;
				start_capital = capital_amount;
				slippage = slippage_amount;
				fees = fees_percent;
				selected_strategy = strat;
		}

}
