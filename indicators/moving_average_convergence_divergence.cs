using static Logger;
//MACD
//shows the relationship between two exponential moving averages
public class MACD : IIndicator
{

		public string name { get; set; }
		int signal_length;

		//indicators
		exponential_moving_average ema_short;
		exponential_moving_average ema_long;
		exponential_moving_average ema_signal;

		//variables
		public decimal ema_short_result;
		public decimal ema_long_result;
		public decimal macd_line;
		public decimal macd_signal_line;
		public decimal histogram;
		bool initiated = false;

		public void update(Candle c)
		{
				//update indicators
				ema_short.update(c);
				ema_long.update(c);

				//get results
				ema_short_result = ema_short.get_result()["Median"];
				ema_long_result = ema_long.get_result()["Median"];

				//calculate difference between emas
				if (ema_short_result != 0 && ema_long_result != 0)
				{
					macd_line = ema_short_result - ema_long_result;
					Candle part_candle = new Candle();
					part_candle.close = macd_line;
					ema_signal.update(part_candle);
				}


				macd_signal_line = ema_signal.get_result()["Median"];

				if (macd_signal_line != 0)
				{
					histogram =  macd_line - macd_signal_line;
					if (!initiated)
					{
						initiated = true;
						log($"Indicator {name} is now ready","INDICATORS");
					}
				}
				else
				{
					log($"Indicator[{name}] is still in warmup","INDICATORS");
				}

		}

		public Dictionary<string, decimal> get_result()
		{
				Dictionary<string, decimal> result = new Dictionary<string, decimal>();
				result.Add("macd_line", macd_line);
				result.Add("signal_line",macd_signal_line);
				result.Add("histogram", histogram);

				return result;
		}
		public MACD(string name, int length_ema_short, int length_ema_long, int length_signal_line)
		{
				this.name = name;
				this.signal_length = length_signal_line;
				ema_long = new exponential_moving_average(length_ema_long, "ema_long_macd");
				ema_short = new exponential_moving_average(length_ema_short, "ema_short_macd");
				ema_signal = new exponential_moving_average(signal_length, "signal_line_ema");
		}

}
