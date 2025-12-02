using static Logger;
public class exponential_moving_average : IIndicator
{
		//name of the indicator
		public string name { get; set; }
		decimal length { get; set; }

		//get an SMA
		moving_average sma;

		//variables
		decimal result = 0;
		int history = 0;
		decimal smoothing;

		//update the indicator
		public void update(Candle c)
		{
				//calculate smoothing
				if (smoothing == 0)
				{
						smoothing = 2 / (length + 1);
				}

				//update SMA first
				sma.update(c);
				history++;

				//if SMA ready calcualte EMA
				if (history >= length)
				{
						if (result == 0)
						{
								result = sma.get_result()["Median"];
						}
						else
						{

								result = smoothing * c.close + result * (1 - smoothing);

						}
				}
				else
				{
						log($"EMA:{name} is still in warmup [{history}] of {length} ","INDICATORS");
				}

		}


		public Dictionary<string, decimal> get_result()
		{
				Dictionary<string, decimal> output = new Dictionary<string, decimal>();
				output.Add("Median", result);

				return output;
		}


		//constructor
		public exponential_moving_average(int length, string name)
		{
				this.name = name;
				this.length = length;
				this.sma = new moving_average(length, "SMA_for_EMA");
		}

}
