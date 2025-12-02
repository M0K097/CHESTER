//Copyright (c) 2025 Moritz Kolb
//Licensed for non-commercial use only. See LICENSE file for details.
//---------------->>>API_MANAGER<<<--------------//oo>>
using Binance.Net.Clients;
using Binance.Net.Objects.Models.Spot;
using Binance.Net.Enums;
using static Logger;
using static Broker;

public static class api_manager
{
		static BinanceRestClient client = new BinanceRestClient();

		async public static Task<BinanceExchangeInfo> get_exchange_data()
		{
				log("fetching exchange information...","API_MANAGER");
				var info = await client.SpotApi.ExchangeData.GetExchangeInfoAsync();

				if (info.Success)
				{
						log($"successfully fetched data from {info.DataSource}: [{client.Exchange}] at server-time: [{info.Data.ServerTime}]","API_MANAGER");
				}
				else
				{
						log("Error while fetching exchange data!","ERROR");
						if (info.Error != null)
						{
								log("Error Message = " + info.Error.Message,"ERROR");
						}
						else
						{
								log("Fatal: Error Object is Null!","ERROR");
						}
				}

				return info.Data;
		}


		static async public Task<decimal?> get_total_symbol_balance(string symbol_pair)
		{
				BinanceExchangeInfo? exchange_info = null; 
				do
				{
					exchange_info = await get_exchange_data();
					if (exchange_info != null)
					{
						log("$ERROR => exchange_data was null","ERROR");
					}
				}
				while(exchange_info == null);

				var symbol_pairs = get_symbols(exchange_info);
				var my_pair = symbol_pairs.Where(symbol => symbol.Name == symbol_pair);
				string quote_asset = my_pair.First().QuoteAsset;
				string base_asset = my_pair.First().BaseAsset;

				decimal? quote_amount = await get_balance_of(quote_asset);
				decimal? base_amount = await get_balance_of(base_asset);
				decimal? base_price = await get_price_of(symbol_pair);


				decimal? total_amount = base_amount * base_price + quote_amount; 

				return total_amount;


		}

		async static public Task<decimal> get_price_of(string symbol)
		{
				decimal price = 0;
				var result = await client.SpotApi.ExchangeData.GetPriceAsync(symbol);
				log($"trying to fetch price for symbol [{symbol}]","API_MANAGER");

				if (result.Success)
				{
						log($"sucessfully fetched price of symbol: [{symbol}] price = {result.Data.Price}","API_MANAGER");
						price = result.Data.Price;
				}
				else
				{
						log($"Error while fetching price! Message:{result.Error}","ERROR");
				}

				return price;
		}


		static public IEnumerable<BinanceSymbol> get_symbols(BinanceExchangeInfo data)
		{
				log("loading symbols...","API_MANAGER");
				IEnumerable<BinanceSymbol>? symbols = data.Symbols;
				return symbols;
		}

		async public static Task<IEnumerable<Binance.Net.Interfaces.IBinanceKline>> get_klines(string symbol, DateTime start, DateTime end)
		{
				KlineInterval time_frame = KlineInterval.OneMinute;
				log($"making api call for [{time_frame}] klines for symbol:[{symbol}] Datrange: [from:{start}  to:{end}]", "REST_API");
				var klinge_data = await client.SpotApi.ExchangeData.GetKlinesAsync(symbol, time_frame, start, end);
				var klines = klinge_data.Data;

				if (klines.Count() == 0)
				{
						log("Error!: api call retruned no klines","REST_API");
						throw new Exception("last api call returned no data");
				}
				log($"<= returning [{klines.Count()}] klines", "REST_API");


				return klines;
		}

		public static List<string> print(IEnumerable<BinanceSymbol> symbols)
		{
				List<string> stringed_symbols = new List<string>();
				foreach (BinanceSymbol s in symbols)
				{
						stringed_symbols.Add(s.Name);
				}
				return stringed_symbols;
		}

		async public static Task<List<string>> list_available_symbols(string BaseAsset = "none", string QuoteAsset = "none")
		{
				BinanceExchangeInfo information = await get_exchange_data();
				IEnumerable<BinanceSymbol> symbols = api_manager.get_symbols(information);
				IEnumerable<BinanceSymbol> filtered_symbols = acid_filter.filter(symbols, BaseAsset, QuoteAsset);
				return print(filtered_symbols);
		}
}

