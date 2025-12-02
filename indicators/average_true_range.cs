//ATR = average true range
//calculates the strength of historic volatility


class ATR : IIndicator
{
	public string name {get; set;}
	int length {get;set;}

	//variables
	List<decimal> true_range = new List<Decimal>();
	Candle last_candle = new Candle();
	decimal average_true_range;
	bool initiated = false;
	

	public void update(Candle c)
	{
		if (initiated)
		{
		decimal range_of_this_candle = c.high - c.low;
		decimal range_to_previous_high = Math.Abs(c.high - last_candle.close);
		decimal range_to_previous_low = Math.Abs(c.low - last_candle.close);

		decimal[] tmp = {range_of_this_candle,range_to_previous_high,range_to_previous_low};
		decimal tr = tmp.Max();
		true_range.Add(tr);

		if (true_range.Count() > length)
		{
			true_range.RemoveAt(0);
		}

		average_true_range = true_range.Average();

		}

		last_candle = c;
		initiated = true;

	}

	public Dictionary<string,decimal> get_result()
	{
		Dictionary<string,decimal> result = new Dictionary<string, decimal>();
		result.Add("result",average_true_range);

		return result;
	}

	public ATR(int length, string name)
	{
		this.name = name;
		this.length = length;
	}


}
