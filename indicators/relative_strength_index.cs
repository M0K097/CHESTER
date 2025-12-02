//RSI = relative strength index

class RSI : IIndicator
{
		public string name { get; set; }
		public int length { get; set; }

		//variables
		List<decimal> change = new List<decimal>();
		decimal last_price = 0;
		decimal relative_strength;
		decimal relative_strength_index;

		public void update(Candle c)
		{
				if (last_price != 0)
				{
						decimal price_diff = c.close - last_price;
						change.Add(price_diff);

				}
				if (change.Count() > length)
				{
						change.RemoveAt(0);

						decimal average_gain = 0;
						decimal average_loss = 0;
						foreach (decimal diff in change)
						{
								if (diff > 0)
								{
										average_gain += diff;
								}
								else
								{
										average_loss -= diff;
								}

						}

						if (average_loss != 0)
						{
								relative_strength = average_gain / average_loss;

						}
						else
						{
							relative_strength = average_gain;

						}

						relative_strength_index = 100 - (100 / (1 + relative_strength));


				}

				last_price = c.close;

		}

		public Dictionary<string, decimal> get_result()
		{
				Dictionary<string, decimal> result = new Dictionary<string, decimal>();
				result.Add("index", relative_strength_index);

				return result;

		}

		public RSI(string name, int length)
		{
				this.name = name;
				this.length = length;
		}

}
