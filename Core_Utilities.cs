//Copyright (c) 2025 Moritz Kolb
//Licensed for non-commercial use only. See LICENSE file for details.
using static api_manager;
using static Logger;
using Microsoft.Extensions.Configuration;

public static class core_configurations
{
		public static IConfigurationRoot settings = new ConfigurationBuilder()
		.SetBasePath(Directory.GetCurrentDirectory())
		.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
		.Build();

}


public static class Core_Utilities
{

		static public async Task download_data()
		{
				string selected_asset_pair = "";
				Console.Write("------> enter filter symbol:");
				List<string> available_symbols;
				string? input = Console.ReadLine();
				Console.WriteLine($"filtering for [{input}]...");

				if (input != null && input != "")
				{
						available_symbols = await list_available_symbols(input);
				}
				else
				{
						available_symbols = await list_available_symbols("none");
				}

				if (available_symbols.Count() > 0)
				{
						selected_asset_pair = CLU_UI.run(available_symbols, "SELECT ASSET-PAIR");
						//break out of menu
						if (selected_asset_pair != "return")
				{
    					DateTime[] selected = select_time();
    					DateTime _from = selected[0];
    					DateTime _to = selected[1];

    					candle_manager loader1 = new candle_manager();

    					// Ordner plattformübergreifend erstellen
    					string folder = Path.Combine("data"); // relativer Pfad, funktioniert auf Windows & Linux
    					Directory.CreateDirectory(folder);

    					// Ungültige Zeichen im Dateinamen ersetzen
    					string safeAssetPair = selected_asset_pair.Replace(":", "_").Replace("/", "_");

    					// Pfad zusammenbauen
    					string fileName = $"{safeAssetPair}_{_from:yyyy.MM.dd}-{_to:yyyy.MM.dd}.json";
 						string path = Path.Combine(folder, fileName);

    					// Daten herunterladen
    					_ = loader1.fetch_candles(selected_asset_pair, _from, _to, path);
				}
					
				}
		}

		static DateTime[] select_time()
		{
				DateTime _from = new DateTime();
				DateTime _to = new DateTime();
				bool selecting = true;

				while (selecting)
				{
						try
						{
								//ask for timeframe
								Console.Write("from:");
								_from = Convert.ToDateTime(Console.ReadLine());
								Console.Write("to:");
								_to = Convert.ToDateTime(Console.ReadLine());

						}
						catch (Exception e)
						{
								Console.WriteLine(e);
								continue;
						}

						selecting = false;
				}
				return new DateTime[] { _from, _to };
		}

		static public void do_backtest()
		{

				while (true)
				{
						List<string> files = Directory.GetFiles("data/").ToList();
						string selected_data = CLU_UI.run(files, "SELECT DATA");

						if (selected_data == "return")
						{
								break;
						}

						candle_maker maker = new candle_maker();
						List<Candle> loaded_data = maker.load_from_json(selected_data);

						Console.Write("Enter timeframe in Minutes:");
						List<Candle> transformed_candles;
						while (true)
						{
								try
								{
										int timeframe = Convert.ToInt32(Console.ReadLine());
										transformed_candles = maker.make_size(timeframe, loaded_data);
										break;
								}
								catch (Exception e)
								{
										Console.WriteLine("Error => wrong input \n" +
										$"Errormessage = 	{e}");
										Console.WriteLine("Enter timeframe again: ");

								}
						}

						decimal capital = Convert.ToDecimal(core_configurations.settings["SIMULATOR:start_capital"]);
						decimal fees = Convert.ToDecimal(core_configurations.settings["SIMULATOR:fees"]);
						decimal slippage = Convert.ToDecimal(core_configurations.settings["SIMULATOR:slippage"]);

						IStrategy selected_strategy = Strategy_Manager.select_strategy();
						Simulator backtester = new Simulator(1000, (decimal)0.1, (decimal)0.1, selected_strategy);
						foreach (Candle c in transformed_candles)
						{
								backtester.update(c);
						}
						backtester.show_statistics();
						Console.WriteLine("wanna do monte carlo test? [y/n]");
						var answer = Console.ReadLine();
						if (answer == "y")
						{
								backtester.do_monte_carlo_test();
						}

				}
		}
		public static void select_log()
		{
				List<string> channels = list_channels();
				bool selecting = true;
				while (selecting)
				{
						if (channels.Count() > 0)
						{

								string selected = CLU_UI.run(channels, "LOGS");
								if (selected == "return")
								{
										break;
								}
								Console.Clear();
								List<string> selected_channel = get_logs(selected);
								CLU_UI.run(selected_channel, $"LOGS OF {selected}");
						}
				}
		}
		public static void monitor_instances()
		{
				while (true)
				{

						List<string> monitor_instances = new List<string>();
						foreach (KeyValuePair<string, Status> m in Monitor.all_callbacks)
						{
								monitor_instances.Add(m.Key);

						}
						string selected = CLU_UI.run(monitor_instances, "MONITORS");
						if (selected != "return")
						{
								Monitor.monitor_worker(selected);

						}
						else
						{
								break;
						}

				}
		}
}
