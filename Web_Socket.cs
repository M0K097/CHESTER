using Binance.Net.Clients;
using Binance.Net.Enums;
using static Logger;
using Binance.Net.Interfaces;
using System.Diagnostics;
//-----WEB_SOCKET>>>
//connects to a symbolpair livestream

public class Web_Socket
{

		//create a new client
		BinanceSocketClient client = new BinanceSocketClient();

		//subscribe for stream
		async public Task connect(string symbolpair, Action<IBinanceStreamKline> make_callback)
		{
				log($"starting web socket instance for {symbolpair}", "WEBSOCKET");
				const int timeout = 60000;
				var timer = new Stopwatch();
				timer.Start();

				var result = await client.SpotApi.ExchangeData.SubscribeToKlineUpdatesAsync
					(
					 symbolpair,
					 interval: KlineInterval.OneMinute,
					 onMessage: data =>
					 {

							 log($"time_interval: {timer.ElapsedMilliseconds} ms [{symbolpair}]", $"WEBSOCKET");
							 if (timer.ElapsedMilliseconds > timeout)
							 {
									 log($"last canlde was over {timer.ElapsedMilliseconds} milliseconds", "ERROR");

							 }
							 timer.Restart();


							 //get kline data
							 var kline = data.Data.Data;
							 if (kline.Final)
							 {
									 log($"=> WEBSOCKET received new Kline :  ClosePrice = {kline.ClosePrice}", "WEBSOCKET");
									 make_callback(kline);
							 }
					 });

				//check if success
				if (result.Success)
				{
						log("connection was successfull", "WEBSOCKET");
						log($"{client.Exchange}", "WEBSOCKET");
						log($"Symbolpair: {symbolpair}", "WEBSOCKET");
				}
				else
				{
						log($"Connection error {result.Error}", "ERROR");
				}
		}
		public void stop()
		{
				client.Dispose();
				log($"WEB_SOCKET got disposed", "WEBSOCKET");

		}

}

