using static Logger;
//DC
//donchian channel
public class donchain_channel : IIndicator
{
		//name of the indicator 
		public string name { get; set; }

		//create variables
		int length;
		List<decimal> highs = new List<decimal>();
		List<decimal> lows = new List<decimal>();
		public decimal upper_band { get; set; }
		public decimal lower_band { get; set; }
		public decimal median { get; set; }

		//update indicator
		public void update(Candle c)
		{
				//push new data
				highs.Add(c.high);
				lows.Add(c.low);

				//remove old data if full
				if (highs.Count() > length)
				{
						highs.RemoveAt(0);
						lows.RemoveAt(0);
				}

				//get max values if warm
				if (highs.Count() == length)
				{
						upper_band = highs.Max();
						lower_band = lows.Min();
						median = (upper_band + lower_band) / 2;
				}
				else
				{
						log($"dc still in warmup [{highs.Count()}] of {length}","INDICATORS");
				}

		}

		//get output
		public Dictionary<string, decimal> get_result()
		{
				Dictionary<string, decimal> output = new Dictionary<string, decimal>();

				output.Add("upper band", upper_band);
				output.Add("lower band", lower_band);
				output.Add("median", median);

				return output;

		}

		public donchain_channel(int length, string name)
		{
				this.length = length;
				this.name = name;
		}
}

