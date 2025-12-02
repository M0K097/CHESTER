//Copyright (c) 2025 Moritz Kolb
//Licensed for non-commercial use only. See LICENSE file for details.
using Binance.Net.Objects.Models.Spot;
using Binance.Net.Enums;
using static Logger;
using System.Text.Json;

//____DATA_MANAGER____//
//this module manages all the price data.
//--------------------------------------->>>>>
//-fetch historic or life data from th exchange.
//-safe data to file.
//-convert data to candle objects to be further processed by Chester.
//--------------------------------------->>>>>


//canlde objects for chester
public class Candle
{
		public DateTime open_time { get; set; }
		public DateTime close_time { get; set; }
		public decimal open { get; set; }
		public decimal close { get; set; }
		public decimal high { get; set; }
		public decimal low { get; set; }
		public decimal volume { get; set; }
		public int trades { get; set; }

}


//fetches candles from the exchange
public class candle_manager
{


		//creates chester specific candle objects
		private List<Candle> make(IEnumerable<Binance.Net.Interfaces.IBinanceKline> klines)
		{
				List<Candle> candle_data = new List<Candle>();
				foreach (Binance.Net.Interfaces.IBinanceKline k in klines)
				{

						Candle candle_object = new Candle();

						candle_object.open_time = k.OpenTime;
						candle_object.open = k.OpenPrice;
						candle_object.close = k.ClosePrice;
						candle_object.high = k.HighPrice;
						candle_object.low = k.LowPrice;
						candle_object.close_time = k.CloseTime;
						candle_object.volume = k.Volume;
						candle_object.trades = k.TradeCount;

						candle_data.Add(candle_object);
				}

				return candle_data;

		}

		//overload make method for single candles
		public Candle make(Binance.Net.Interfaces.IBinanceKline k)
		{

				Candle candle_object = new Candle();

				candle_object.open_time = k.OpenTime;
				candle_object.open = k.OpenPrice;
				candle_object.close = k.ClosePrice;
				candle_object.high = k.HighPrice;
				candle_object.low = k.LowPrice;
				candle_object.close_time = k.CloseTime;
				candle_object.volume = k.Volume;
				candle_object.trades = k.TradeCount;

				return candle_object;

		}



		//safe candle objects to json file	
		void safe_to_json(List<Candle> data, string path)
		{
				if (data.Count() > 0)
				{
						string json_data = JsonSerializer.Serialize(data);
						File.WriteAllText(path, json_data);
						log($"safed [{data.Count()}] candles to path: {path}");
				}
				else
				{
						log("Error data had no entries");
				}
		}



		//make multible api calls on agiven timewindow
		async public Task fetch_candles(string symbol, DateTime start_time, DateTime end_time, string safe_path)
		{
				//safe start and end
				DateTime start = start_time;
				DateTime end = end_time;
				DateTime partition_end = new DateTime(0);
				TimeSpan total = end_time - start_time;

				//data
				List<Candle> full_data = new List<Candle> { };

				log($"fetching price data [{symbol}] | from {start_time} - {end_time}");

				try
				{
						while (start < end)
						{

								//calculate difference 
								TimeSpan difference = end - start;

								if (difference.TotalMinutes > 500)
								{
										partition_end = start.AddMinutes(500);
								}
								else if (difference.TotalMinutes > 0)
								{
										partition_end = start.AddMinutes(difference.TotalMinutes);
								}
								List<Candle> part_data = make(await api_manager.get_klines(symbol, start, partition_end));
								start = partition_end;
								full_data.AddRange(part_data);
								await Task.Delay(200);

								TimeSpan partial = end - start;
								double percentage = Math.Floor((1 - partial.TotalSeconds / total.TotalSeconds) * 100);

								log($"download {symbol}: {percentage}%");


						}
						safe_to_json(full_data, safe_path);
				}
				catch (Exception e)
				{
						log($"Error: {e}");
				}

		}
}


//class that makes candle objects from data
public class candle_maker
{
		//loads json data from file
		public List<Candle> load_from_json(string path)
		{
				log("loading up data...");

				string json_loaded = File.ReadAllText(path);
				var data = JsonSerializer.Deserialize<List<Candle>>(json_loaded);

				if (data != null)
				{
						log($"<- loaded [{data.Count()}] candle objects");
						return data;

				}

				throw new NullReferenceException("data cant be null");
		}

		//makes bigger candles out of small ones
		public List<Candle> make_size(int minutes, List<Candle> data)
		{


				//make new list
				List<Candle> transformed_list = new List<Candle> { };

				for (int i = 0; i + minutes < data.Count() + 1; i += minutes)
				{

						List<Candle> builder = new List<Candle> { };

						for (int y = 0; y < minutes; y++)
						{
								builder.Add(data[i + y]);
						}

						//build a new candle
						Candle aggregated_candle = new Candle { };

						aggregated_candle.open_time = builder[0].open_time;
						aggregated_candle.open = builder[0].open;
						aggregated_candle.high = builder.Max(c => c.high);
						aggregated_candle.low = builder.Min(c => c.low);
						aggregated_candle.close = builder[minutes - 1].close;
						aggregated_candle.close_time = builder[minutes - 1].close_time;
						aggregated_candle.trades = builder.Sum(c => c.trades);
						aggregated_candle.volume = builder.Sum(c => c.volume);

						//push it to  a new list
						transformed_list.Add(aggregated_candle);

				}

				return transformed_list;
		}

}

public static class acid_filter
{

		//function that filters given asset symbols
		static public IEnumerable<BinanceSymbol> filter(IEnumerable<BinanceSymbol> symbols_list, string base_asset = "none", string quote_asset = "none")
		{
				log($"filtering symbols with parameters: [BaseAsset: {base_asset}]  [QuoteAsset: {quote_asset}]");
				//--->>hard filters
				symbols_list = symbols_list.Where(s => s.IsSpotTradingAllowed == true);
				symbols_list = symbols_list.Where(s => s.Status == SymbolStatus.Trading);

				//--->>parameter filters	
				if (base_asset != "none")
				{
						symbols_list = symbols_list.Where(s => s.BaseAsset == base_asset);
				}
				if (quote_asset != "none")
				{
						symbols_list = symbols_list.Where(s => s.QuoteAsset == quote_asset);
				}
				log($"<= returning {symbols_list.Count()} symbols");
				return symbols_list;

		}



}
