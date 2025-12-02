using System.Collections.Concurrent;
using static Logger;

public class Status
{
		string name { get; set; }
		DateTime start_time { get; set; }
		public DateTime last_update { get; set; }
		public Chester WORKING_CHESTER { get; set; }
		public string option { get; set; }

		public void log_status()
		{

				Chester w = WORKING_CHESTER;
				string status_info = ($"\nNAME: {name}\n" +
						$"STRATEGY: {w.strategy}\n" +
						$"START_TIME: {start_time}\n" +
						$"LAST_UPDATE: {last_update}\n" +
						$"SYMBOLPAIR: {w.symbol_pair}\n" +
						$"CANDLESIZE: {w.candle_size}\n" +
						$"OPTION: {w.option}\n");

				var p = w.papertrader;
				var l = w.live_performance_monitor;

				if (w.option == "PAPERTRADER" && p.trades > 0)
				{
						string status_info_more = ($"\nSINGLE_TRADES: {p.trades}\n" +
								$"WARMUP_TIME: {p.warmup}\n" +
								$"PROCESSED_CANDLES: {p.data.Count()}\n" +
								$"START CAPITAL: {p.equity_curve.First()}\n" +
								$"TOTAL CAPITAL: {p.total_capital}\n" +
								$"CAPITAL PERFORMANCE: {Math.Round(((p.total_capital / p.equity_curve.First())) * 100, 3)}%\n" +
								$"QUOTE_ASSET: {p.quote_asset}\n" +
								$"BASE_ASSET: {p.base_asset}");

						status_info += status_info_more;

				}

				else if (w.option == "BROKER" && l.capital.Count() > 0 && l.capital.First() != 0)
				{

						string status_info_more = ($"\nWARMUP_TIME: {w.warmup}\n" +
								$"CHESTER_UPDATE_COUNTER: {w.counter}\n" +
								$"ANALYZER_PROCESSED_CANDLES: {l.time.Count()}\n" +
								$"CAPITAL START: {l.capital.First()}\n" +
								$"CAPITAL NOW: {l.capital.Last()}\n" +
								$"CAPITAL PERFORMANCE: {Math.Round(((l.capital.Last() / l.capital.First())) * 100, 3)}%\n" +
								$"SYMBOLPAIR: {w.symbol_pair}");


						status_info += status_info_more;
				}
				else
				{
						string status_info_empty = (" => no trades to calculate detailed performance");
						status_info += status_info_empty;
				}
				Console.WriteLine(status_info);
				Console.WriteLine("Press Enter...");
				Console.ReadLine();

		}

		public Status(string name, DateTime start_time, Chester working_chester)
		{
				this.name = name;
				this.start_time = start_time;
				this.WORKING_CHESTER = working_chester;
				this.option = working_chester.option;
		}
}

static class Monitor
{
		public static ConcurrentDictionary<string, Status> all_callbacks = new ConcurrentDictionary<string, Status>();
		static public void monitor_worker(string name_of_worker)
		{
				Status worker = all_callbacks[name_of_worker];
				worker.log_status();

				Chester w = worker.WORKING_CHESTER;

				if (w.option == "PAPERTRADER" && w.papertrader.trades > 0)
				{
						log("showing papertrader stats");
						w.papertrader.show_statistics();
				}
				else if (w.option == "BROKER" && w.live_performance_monitor.capital.Count() > 0)
				{
						log("showing live stats");
						var performance = w.live_performance_monitor;
						List<Trade> trades = performance.reproduce_trades();
						performance.make_metrics(trades);
						performance.inspect_trades(trades);

				}

		}
}

