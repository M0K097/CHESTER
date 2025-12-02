using static Logger;
//SMA
//simple moving average
public class moving_average : IIndicator
{
		//indicators name
		public string name { get; set; }

		//variables
		int length;
		List<decimal> data = new List<decimal>();
		public decimal result;

		//update the indicator
		public void update(Candle c)
		{
				//fill with data
				data.Add(c.close);
				if (data.Count() > length)
				{
						data.RemoveAt(0);
				}

				//return if warmup completet
				if (data.Count() == length)
				{
						result = data.Average();
				}
				else
				{
						log($"MA is in warmup:  [{data.Count()}] from {length} candles","INDICATORS");
				}
		}

		//return output
		public Dictionary<string, decimal> get_result()
		{
				Dictionary<string, decimal> output = new Dictionary<string, decimal>();
				output.Add("Median", result);

				return output;

		}

		//constructor
		public moving_average(int length, string name)
		{
				this.length = length;
				this.name = name;
		}
}
