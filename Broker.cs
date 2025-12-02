//Copyright (c) 2025 Moritz Kolb
//Licensed for non-commercial use only. See LICENSE file for details.
//manages real time orders
using Binance.Net.Clients;
using CryptoExchange.Net.Authentication;
using static Logger;
using static api_manager;


public static class Broker
{


		static BinanceRestClient broker = new BinanceRestClient(options =>
				{
						
						var data = core_configurations.settings;
						var api_key = data["BROKER:key"]; 
						var api_secret = data["BROKER:secret"]; 

						log("broker is connecting with given credentials...", "BROKER");

						
						if (api_key != null && api_secret != null)
						{
							
							options.ApiCredentials = new ApiCredentials
							(
								api_key.Trim(),
								api_secret.Trim()
							);

						}
						else
						{
							log("one of the credentials was null", "ERROR");
							log("one of the credentials was null", "BROKER");
						}

						log("broker with credentials was initiated", "BROKER");

				});



		async public static Task place_order(Binance.Net.Enums.OrderSide side, string symbol_pair, decimal amount_in_percent)
		{

				var exchange_info = await get_exchange_data();
				var symbol_pairs = get_symbols(exchange_info);
				var my_pair = symbol_pairs.Where(symbol => symbol.Name == symbol_pair);

				string quote_asset = my_pair.First().QuoteAsset;
				string base_asset = my_pair.First().BaseAsset;

				log($"calculating for: QuoteAsset:[{quote_asset}] --- BaseAsset[{base_asset}]", "BROKER");
				int rounding_parameter = my_pair.First().QuoteAssetPrecision;
				decimal? quantity = 0;

				if (amount_in_percent > 0)
				{
						var free = await get_balance_of(quote_asset);
						quantity = free * amount_in_percent;
						log($"total_available_amount = {free} | strategy_return_percentage = {amount_in_percent} => trying to buy quantity:{quantity} of QuoteAsset:[{quote_asset}]", "BROKER");

				}
				else
				{
						amount_in_percent *= -1;

						var free = await get_balance_of(base_asset);
						var price_baseAsset = await get_price_of(symbol_pair);

						quantity = free * amount_in_percent * price_baseAsset;
						log($"total_available_amount = {free} | strategy_return_percentage = {amount_in_percent} => trying to sell quantity:{quantity} of QuoteAsset:[{base_asset}]", "BROKER");



				}


				decimal new_quantity = 0;
				if (quantity != null)
				{
						new_quantity = Convert.ToDecimal(quantity);
				}
				new_quantity = Math.Round(new_quantity, rounding_parameter);
				log($"rounding_parameter = {rounding_parameter}","BROKER");
				log($"calculated new quantity = {new_quantity}","BROKER");
				var result = await broker.SpotApi.Trading.PlaceOrderAsync(
				symbol: symbol_pair,
				side: side,
				type: Binance.Net.Enums.SpotOrderType.Market,
				quoteQuantity: new_quantity
				);

				if (result.Success)
				{
						log($"order id: {result.Data.Id}","BROKER");
						log($"symbol: {result.Data.Symbol}","BROKER");
						log($"quantity: {result.Data.Quantity}","BROKER");
						log($"price: {result.Data.Price}","BROKER");
						log($"order_side: {result.Data.Side}","BROKER");
				}
				else
				{
						log($"ERROR: {result.Error}","ERROR");
				}

				}

				public async static Task execute_order(int result, string symbol_pair)
				{
				
				decimal result_in_decimal = Convert.ToDecimal(result);
		
				log($"trying to buy {result}%", "INSTANCES");
				decimal result_to_percent = result_in_decimal / 100;


				if (result > 0)
				{
						log($"goona buy {result}% of {symbol_pair}", "BROKER");
					        await place_order(Binance.Net.Enums.OrderSide.Buy, symbol_pair, result_to_percent);

				}
				else if (result < 0)
				{
						log($"gonna sell {result}% of {symbol_pair}", "BROKER");
						await place_order(Binance.Net.Enums.OrderSide.Sell, symbol_pair, result_to_percent);
				}
				}	

		async public static Task<decimal?> get_balance_of(string asset)
		{

				var account_data = await broker.SpotApi.Account.GetAccountInfoAsync();
				log(account_data.ToString(),"BROKER");
				decimal? free_capital = 0;
				decimal minimum_available = 0;

				if (account_data.Success)
				{
						var balance = account_data.Data.Balances.FirstOrDefault(b => b.Asset == asset);
						free_capital = balance?.Available;

						if (free_capital >= minimum_available)
						{
								log($"available capital = {free_capital}","BROKER");

						}
						else
						{
								log($"available capital {free_capital} is < {minimum_available}", "ERROR");
						}

				}
				else
				{
						log($"ERROR: {account_data.Error?.Message}", "ERROR");
				}

				return free_capital;

		}


}
