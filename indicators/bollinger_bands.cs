using static Logger;
//BB = Bollinger Bands
//calculates the X * standart diviation of an assets mean price

public class BB : IIndicator
{
		//initiation
		public string name { get; set; }
		int length { get; set; }
		List<decimal> close_prices = new List<decimal>();
		bool initiated = false;
		moving_average ma { get; set; }
		decimal standart_deviation = 0;
		decimal ma_result = 0;

		public void update(Candle c)
		{
				ma.update(c);
				close_prices.Add(c.close);

				ma_result = ma.get_result()["Median"];

				if (ma_result != 0)
				{
						List<double> all_squared_means = new List<double>();
						foreach (decimal price in close_prices.ToList())
						{
								if (close_prices.Count() >= length)
								{
										close_prices.RemoveAt(0);
								}

								decimal price_minus_mean = price - ma.result;
								double squared_mean = Math.Pow((double)price_minus_mean, 2.0);
								all_squared_means.Add(squared_mean);

						}

						var average_of_all_squared_results = all_squared_means.Sum() / all_squared_means.Count();
						var square_root_of_all_squared_variances = Math.Sqrt(average_of_all_squared_results);
						standart_deviation = Convert.ToDecimal(square_root_of_all_squared_variances);
						all_squared_means.Clear();

				}
				else
				{
						log($"bollinger_bands [{name}] are still waiting for sma to warm up {close_prices.Count()} / {length}");
				}
		}


		public Dictionary<string, decimal> get_result()
		{
				Dictionary<string, decimal> result = new Dictionary<string, decimal>();

				result.Add("Median", ma_result);
				result.Add("Upper_Band", ma_result + standart_deviation * 2);
				result.Add("Lower_Band", ma_result - standart_deviation * 2);

				return result;
		}

		public BB(int length, string name)
		{
				this.name = name;
				this.length = length;
				this.ma = new moving_average(length, "bb_ma");
		}


}
